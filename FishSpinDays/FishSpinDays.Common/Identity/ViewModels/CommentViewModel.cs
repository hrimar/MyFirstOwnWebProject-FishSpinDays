﻿namespace FishSpinDays.Common.Identity.ViewModels
{
    using System;

    public class CommentViewModel
    {
        
        public int Id { get; set; }

        public string Text { get; set; }

        public string Author { get; set; }

        public string Publication { get; set; }

        public DateTime CreationDate { get; set; }

        public int Likes { get; set; }

        public int UnLikes { get; set; }
    }
}
