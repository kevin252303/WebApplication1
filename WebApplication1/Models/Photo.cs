﻿using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public int AppUserId { get; set; }
        [ForeignKey("AppUserId")]
        public AppUsers User { get; set; }
    }
}