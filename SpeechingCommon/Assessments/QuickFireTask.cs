using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpeechingCommon
{
    public class QuickFireTask : IAssessmentTask
    {
        private int id;
        private string title;
        private string instructions;

        public string[] Prompts;

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        public string Instructions
        {
            get
            {
                return this.instructions;
            }
            set
            {
                this.instructions = value;
            }
        }

    }
}