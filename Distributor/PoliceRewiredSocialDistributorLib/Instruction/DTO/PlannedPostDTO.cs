using System;
namespace PoliceRewiredSocialDistributorLib.Instruction.DTO
{
    public class PlannedPostDTO
    {
        public PlannedPostDTO() { }

        public PlannedPostDTO(SocialPostSheetDTO dto)
        {
            ListId = dto.ListId;
            Text = dto.Text;
            Tags = dto.Tags;
            ImageUrl = dto.ImageURL;
            LinkUrl = dto.URL;
        }

        public string ListId { get; set; }
        public string Text { get; set; }
        public string Tags { get; set; }
        public string LinkUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}
