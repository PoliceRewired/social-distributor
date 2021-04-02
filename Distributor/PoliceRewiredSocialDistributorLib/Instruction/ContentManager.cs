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
        List<SocialPostSheetDTO> socialPosts;

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

        public async Task<PlannedPostDTO> RunAsync(bool writeBack)
        {
            var list = await SelectListAsync();
            var post = await PopNextMessageAsync(list, writeBack);
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

        private async Task<PlannedPostDTO> PopNextMessageAsync(string list, bool writeBack)
        {
            var state = await GetStateAsync();
            if (state == null) { state = new ContentPlanDTO(); }
            if (state.ListPlan == null) { state.ListPlan = new Dictionary<string, List<PlannedPostDTO>>(); }
            if (!state.ListPlan.ContainsKey(list)) { state.ListPlan.Add(list, new List<PlannedPostDTO>()); }
            if (state.ListPlan[list] == null) { state.ListPlan[list] = new List<PlannedPostDTO>(); }
            if (state.ListPlan[list].Count() == 0)
            {
                state.ListPlan[list] = await RecalculatePlanAsync(list);
                Log("Recalculated plan for: " + list);
                Log(JsonConvert.SerializeObject(state.ListPlan[list]));
            }

            PlannedPostDTO next;
            if (state.ListPlan[list].Count() > 0)
            {
                next = state.ListPlan[list].FirstOrDefault();
                state.ListPlan[list].RemoveAt(0);
                Log(string.Format("Retrieved next post for: {0}", next.ListId));
            }
            else
            {
                Log(string.Format("Null next post. No entries for list: {0}", list));
                next = null;
            }

            if (writeBack)
            {
                await WriteBackStateAsync(state);
            }

            return next;
        }

        public async Task<ContentPlanDTO> RecalculateAsync(bool writeBack)
        {
            var rules = await GetListRulesAsync();
            var lists = rules.Select(r => r.ListId.ToLower().Trim());

            var state = new ContentPlanDTO();
            state.ListPlan = new Dictionary<string, List<PlannedPostDTO>>();

            foreach (var list in lists)
            {
                state.ListPlan[list] = await RecalculatePlanAsync(list);
                Log("Recalculated plan for: " + list);
                Log(JsonConvert.SerializeObject(state.ListPlan[list]));
            }

            if (writeBack)
            {
                await WriteBackStateAsync(state);
            }

            return state;
        }

        public async Task ClearAsync(bool writeBack)
        {
            var state = new ContentPlanDTO();

            if (writeBack)
            {
                await WriteBackStateAsync(state);
            }
        }

        private async Task<List<PlannedPostDTO>> RecalculatePlanAsync(string list)
        {
            Log(string.Format("Recalculating plan for: {0}", list));
            var posts = await GetPostsAsync();
            var listPosts = posts.Where(p => p.ListId.ToLower().Trim() == list.ToLower().Trim());

            // randomise list and return in plan form
            return listPosts
                .Select(p => new PlannedPostDTO(p))
                .OrderBy(x => Guid.NewGuid())
                .ToList();
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

        private async Task<List<SocialPostSheetDTO>> GetPostsAsync()
        {
            if (socialPosts == null)
            {
                socialPosts = await CsvWebHelper.ReadDataAsync<SocialPostSheetDTO>(postsCsvUrl);
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
