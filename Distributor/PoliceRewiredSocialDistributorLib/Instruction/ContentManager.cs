using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CsvHelper;
using Newtonsoft.Json;
using PoliceRewiredSocialDistributorLib.Helpers;
using PoliceRewiredSocialDistributorLib.Instruction.DTO;

namespace PoliceRewiredSocialDistributorLib.Instruction
{
    public class ContentManager : IDisposable
    {
        string postsCsvUrl;
        string rulesCsvUrl;
        string stateS3Key;
        string stateS3Bucket;
        S3Helper s3;
        Action<string> log;

        ContentPlanDTO contentPlanState;
        List<SocialListRuleDTO> listRules;
        List<SocialPostDTO> socialPosts;

        public ContentManager(string postsCsvUrl, string rulesCsvUrl, string stateBucket, string stateKey, Action<string> log, RegionEndpoint region = null)
        {
            this.postsCsvUrl = postsCsvUrl;
            this.rulesCsvUrl = rulesCsvUrl;
            this.stateS3Key = stateKey;
            this.stateS3Bucket = stateBucket;
            this.s3 = new S3Helper(stateS3Bucket, log, region);
            this.log = log;
        }

        public void Dispose()
        {
            s3.Dispose();
            s3 = null;
        }

        private void Log(string msg) => log(msg);

        public async Task<ListContentDTO> RunAsync()
        {
            var list = await SelectListAsync();
            var post = await PopNextMessageAsync(list);
            return post;
        }

        private async Task<string> SelectListAsync()
        {
            var rules = await GetListRulesAsync();
            var all = rules.SelectMany(dto => Enumerable.Repeat(dto.ListId, dto.Ratio));
            var rand = new Random();
            var index = rand.Next(all.Count());
            var list = all.ElementAt(index);
            return list;
        }

        private async Task<ListContentDTO> PopNextMessageAsync(string list)
        {
            var state = await GetStateAsync();
            if (state == null) { state = new ContentPlanDTO(); }
            if (state.ListPlan == null) { state.ListPlan = new Dictionary<string, List<ListContentDTO>>(); }
            if (!state.ListPlan.ContainsKey(list)) { state.ListPlan.Add(list, new List<ListContentDTO>()); }
            if (state.ListPlan[list] == null) { state.ListPlan[list] = new List<ListContentDTO>(); }
            if (state.ListPlan[list].Count() == 0)
            {
                state.ListPlan[list] = await RecalculatePlanAsync(list);
            }

            ListContentDTO next;
            if (state.ListPlan[list].Count() > 0)
            {
                next = state.ListPlan[list].FirstOrDefault();
                state.ListPlan[list].RemoveAt(0);
                Log(string.Format("Retrieved next post: {0}/{1}", next.List, next.Content));
            }
            else
            {
                Log(string.Format("Null next post. No entries for list: {0}", list));
                next = null;
            }

            await WriteBackStateAsync(state);

            return next;
        }

        private async Task<List<ListContentDTO>> RecalculatePlanAsync(string list)
        {
            Log(string.Format("Recalculating plan for: {0}", list));
            var posts = await GetPostsAsync();
            var listPosts = posts.Where(p => p.ListId.ToLower().Trim() == list.ToLower().Trim());

            // randomise list and return in plan form
            return listPosts
                .OrderBy(x => Guid.NewGuid())
                .Select(p => new ListContentDTO { List = p.ListId, Content = p.Content }).ToList();
        }

        private async Task<ContentPlanDTO> GetStateAsync()
        {
            if (contentPlanState == null)
            {
                contentPlanState = await s3.ReadObjectFromJsonAsync<ContentPlanDTO>(stateS3Key);
                Log("State: " + JsonConvert.SerializeObject(contentPlanState));
            }
            return contentPlanState;
        }

        private async Task WriteBackStateAsync(ContentPlanDTO state)
        {
            await s3.WriteObjectToJsonAsync(stateS3Key, state);
        }

        private async Task<List<SocialPostDTO>> GetPostsAsync()
        {
            if (socialPosts == null)
            {
                socialPosts = await CsvWebHelper.ReadDataAsync<SocialPostDTO>(postsCsvUrl);
                Log(string.Format("Retrieved {0} posts.", socialPosts.Count()));
            }
            return socialPosts;
        }

        private async Task<List<SocialListRuleDTO>> GetListRulesAsync()
        {
            if (listRules == null)
            {
                listRules = await CsvWebHelper.ReadDataAsync<SocialListRuleDTO>(rulesCsvUrl);
                Log(string.Format("Retrieved {0} rules.", listRules.Count()));
            }
            return listRules;
        }


    }
}
