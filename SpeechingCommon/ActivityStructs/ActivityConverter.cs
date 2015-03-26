using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;

namespace SpeechingCommon
{
    class ActivityConverter : JsonCreationConverter<ISpeechingActivityItem>
    {
        protected override ISpeechingActivityItem Create(Type objectType, JObject jObject)
        {
            if (FieldExists("Tasks", jObject))
            {
                return new Scenario();
            }
            else if (FieldExists("Guides", jObject))
            {
                return new Guide();
            }
            else
            {
                return new Scenario();
            }
        }

        private bool FieldExists(string fieldName, JObject jObject)
        {
            return jObject[fieldName] != null && jObject[fieldName].Any();
        }
    }
}