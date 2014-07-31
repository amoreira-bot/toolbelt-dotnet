using System.Collections.Generic;

namespace Vtex.Toolbelt
{
    public class PromptConfiguration : List<PromptConfiguration.PromptOption>
    {
        public PromptConfiguration Add(char key, string description, bool isDefault = false)
        {
            key = char.ToUpperInvariant(key);
            this.Add(new PromptOption(key, description, isDefault));
            return this;
        }

        public class PromptOption
        {
            public char Key { get; set; }

            public string Description { get; set; }

            public bool IsDefault { get; set; }

            public PromptOption(char key, string description, bool isDefault)
            {
                Key = key;
                Description = description;
                IsDefault = isDefault;
            }
        }
    }
}