using System;
using System.Collections.Generic;
using System.Text;

namespace StartupsFront.Models
{
    public class StartupModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime StartupPublished { get; set; }

        public int AuthorForeignKey { get; set; }

        public string StartupPicFileName { get; set; } = string.Empty;
        public string Viewers { get; set; } = string.Empty;
        public DateTime LastModify { get; set; }

        public List<int> Contributors { get; set; } = new List<int>();
    }
}
