using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LiveSplit.DarkSoulsIGT
{
    public static class ItemProperties
    {
        class ConfigReader
        {
            string text;
            int index;

            internal ConfigReader(string filename)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                StreamReader streamReader = new StreamReader(assembly.GetManifestResourceStream(filename));
                text = streamReader.ReadToEnd();
                index = 0;
            }

            internal bool HasNext()
            {
                return index < text.Length;
            }

            bool IsDigit(char c)
            {
                return c >= '0' && c <= '9';
            }

            bool IsWhitespace(char c)
            {
                return c == ' ' || c == '\t' || c == '\n' || c == '\r';
            }

            bool IsNewline(char c)
            {
                return c == '\n' || c == '\r';
            }

            void SkipWhitespace()
            {
                while (HasNext() && IsWhitespace(text[index]))
                {
                    index++;
                }
            }

            internal int ReadInt32()
            {
                SkipWhitespace();

                int result = 0;
                while (HasNext() && IsDigit(text[index]))
                {
                    int digit = text[index++] - '0';
                    result = result * 10 + digit;
                }
                return result;
            }

            internal string ReadUntilNewline()
            {
                SkipWhitespace();

                int offset = index;
                while (HasNext() && !IsNewline(text[index]))
                {
                    index++;
                }

                int length = index - offset;

                return text.Substring(offset, length);
            }
        }

        // Item config files made by JKAnderson https://github.com/JKAnderson

        public static readonly List<ItemFlag> Armor;
        public static readonly List<ItemFlag> Consumables;
        public static readonly List<ItemFlag> KeyItems;
        public static readonly List<ItemFlag> MeleeWeapons;
        public static readonly List<ItemFlag> RangedWeapons;
        public static readonly List<ItemFlag> Rings;
        public static readonly List<ItemFlag> Shields;
        public static readonly List<ItemFlag> Spells;
        public static readonly List<ItemFlag> SpellTools;
        public static readonly List<ItemFlag> UpgradeMaterials;
        public static readonly List<ItemFlag> UsableItems;


        static ItemProperties()
        {
            Armor = ParseItemProperties("Armor.txt");
            Consumables = ParseItemProperties("Consumables.txt");
            KeyItems = ParseItemProperties("KeyItems.txt");
            MeleeWeapons = ParseItemProperties("MeleeWeapons.txt");
            RangedWeapons = ParseItemProperties("RangedWeapons.txt");
            Rings = ParseItemProperties("Rings.txt");
            Shields = ParseItemProperties("Shields.txt");
            Spells = ParseItemProperties("Spells.txt");
            SpellTools = ParseItemProperties("SpellTools.txt");
            UpgradeMaterials = ParseItemProperties("UpgradeMaterials.txt");
            UsableItems = ParseItemProperties("UsableItems.txt");
        }

        static List<ItemFlag> ParseItemProperties(string filename)
        {
            List<ItemFlag> flags = new List<ItemFlag>();

            ConfigReader reader = new ConfigReader("LiveSplit.DarkSoulsIGT.Resources.Items." + filename);
            while (reader.HasNext())
            {
                int id = reader.ReadInt32();
                int digit0 = reader.ReadInt32();
                int digit1 = reader.ReadInt32();
                string name = reader.ReadUntilNewline();

                flags.Add(new ItemFlag(name, id));
            }

            return flags;
        }
    }
}
