using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CampusLearn_Web_App.Services
{
    public class ContentFilterService
    {
        // Forbidden words or phrases (you can expand this)
        private readonly List<string> _bannedWords = new()
        {
           // 🔞 NSFW / Explicit sexual acts
"porn", "nude", "erotic", "fetish", "masturbate", "orgasm",
"xxx", "escort", "blowjob", "handjob", "threesome", "kamasutra",
"sexual act", "nsfw", "hentai", "incest", "rape", "molest",

// ⚔️ Violence / Self-harm (severe or explicit)
"kill someone", "murder", "stab", "commit suicide", "self harm",
"hang myself", "slit wrists", "torture", "massacre",

// 💊 Drugs / Abuse (recreational or illegal)
"buy drugs", "sell drugs", "snort cocaine", "shoot heroin",
"take lsd", "overdose", "get high",

// 💣 Hate / Extremism
"nazi", "kkk", "white power", "ethnic cleansing",
"homophobic slur", "antisemitic", "racist attack",

// ⚖️ Illegal / Unethical activity
"child porn", "bestiality", "zoophilia", "underage porn",
"pedophile", "pedophilia", "ddos attack", "hack credit card",
"buy weapons", "dark web", "human trafficking",

// 💬 Extremely sensitive events
"beheading video", "school shooting", "mass shooting", "terrorist attack"

        };

        public bool IsMessageAllowed(string message, out string reason)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                reason = "Empty message.";
                return false;
            }

            // Case-insensitive search for banned words
            foreach (var word in _bannedWords)
            {
                if (Regex.IsMatch(message, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase))
                {
                    reason = $"Your message contains a restricted term: '{word}'.";
                    return false;
                }
            }

            reason = string.Empty;
            return true;
        }
    }
}
