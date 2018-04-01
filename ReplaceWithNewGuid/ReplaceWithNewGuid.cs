using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ReplaceGuid
{
    public class ReplaceWithNewGuid
    {
        /// <summary>
        /// delegate for creating new GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public delegate Guid NewGuid();

        /// <summary>
        /// hold delegate NewGuid
        /// </summary>
        private NewGuid delegateNewGuid;

        /// <summary>
        /// dictionary for storing previous translation of GUIDs.
        /// </summary>
        private Dictionary<string, Guid> dict = new Dictionary<string, Guid>();

        /// <summary>
        /// Variable
        /// </summary>
        static private string raw_variable = @"(<*)\w+(>*)";

        /// <summary>
        /// 00000000-0000-0000-0000-000000000000
        /// </summary>
        static private string raw_guid_string1 = "([0-9A-Fa-f]{8})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{4})-([0-9A-Fa-f]{12})";

        /// <summary>
        /// 00000000000000000000000000000000
        /// </summary>
        static private string raw_guid_string2 = "([0-9A-Fa-f]{32})";

        /// <summary>
        /// 0x00000000
        /// </summary>
        static private string hex_4byte_string = "(0[xX][0-9A-Fa-f]{1,8})";

        /// <summary>
        /// 0x0000
        /// </summary>
        static private string hex_2byte_string = "(0[xX][0-9A-Fa-f]{1,4})";

        /// <summary>
        /// 0x00
        /// </summary>
        static private string hex_1byte_string = "(0[xX][0-9A-Fa-f]{1,2})";
        static private string spaces = @"\s*";
        static private string comma_spaces = spaces + "," + spaces;
        //static private string raw_head_separater = @"[{""]";
        //static private string raw_tail_separater = @"[}""]";

        /// <summary>
        /// 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
        /// </summary>
        static private string hex_8_of_1byte = hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string
                                     + comma_spaces + hex_1byte_string;

        /// <summary>
        /// {0x00000000, 0x0000, 0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
        /// </summary>
        static private string raw_string_array = "{"
                                               + spaces
                                               + hex_4byte_string + comma_spaces
                                               + hex_2byte_string + comma_spaces
                                               + hex_2byte_string + comma_spaces
                                               + "{"
                                               + spaces
                                               + hex_8_of_1byte
                                               + spaces
                                               + "}"
                                               + spaces
                                               + "}";

        static private string raw_guid_value = hex_4byte_string + comma_spaces
                                               + hex_2byte_string + comma_spaces
                                               + hex_2byte_string + comma_spaces
                                               + hex_8_of_1byte;
        static private string name_guid_value_def = @"(?<RAW_GUID_DEF>" + raw_guid_value + ")";
        static private string name_guid_value_imp = @"(?<RAW_GUID_IMP>" + raw_guid_value + ")";

        /// <summary>
        /// DEFINE_GUID(<<name>>, 0x00000000,0x0000,0x0000, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);
        /// </summary>
        static private string raw_define_guid = @"(?<="
                                               + @"DEFINE_GUID"
                                               + spaces
                                               + @"\("
                                               + spaces
                                               + raw_variable
                                               + comma_spaces
                                               + @")"
                                               + name_guid_value_def
                                               + @"(?="
                                               + spaces
                                               + @"\)"
                                               + @")";

        /// <summary>
        /// IMPLEMENT_OLECREATE(<<class>>, <<external_name>>, 0x00000000,0x0000,0x0000, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);
        /// </summary>
        static private string raw_impl_olecreate = @"(?<="
                                               + @"IMPLEMENT_OLECREATE"
                                               + spaces
                                               + @"\("
                                               + spaces
                                               + raw_variable
                                               + comma_spaces
                                               + raw_variable
                                               + comma_spaces
                                               + @")"
                                               + name_guid_value_imp
                                               + @"(?="
                                               + spaces
                                               + @"\)"
                                               + @")";

        // For reference
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference#backreference_constructs
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/backreference-constructs-in-regular-expressions
        //static private string name_head_separater = @"(?:" + raw_head_separater + ")";
        //static private string name_tail_separater = @"(?:" + raw_tail_separater + ")";
        static private string word_separater = @"\b";
        static private string name_guid_string1 = @"(?<RawHyphenDigits>" + raw_guid_string1 + ")";
        static private string name_guid_string2 = @"(?<Raw32Digits>"     + raw_guid_string2 + ")";
        static private string name_guid_string3 = @"(?<GuidVariable>" + raw_string_array + ")";
        static private string name_guid_string4 = @"(?<DEFINE_GUID>"  + raw_define_guid    + ")";
        static private string name_guid_string5 = @"(?<OLECREATE>"    + raw_impl_olecreate + ")";
        static private string guid_string1 = word_separater + name_guid_string1 + word_separater;
        static private string guid_string2 = word_separater + name_guid_string2 + word_separater;
        //static private string guid_string3 = word_separater + name_guid_string3 + word_separater;
        static private string[] elements = new string[]
        {
            guid_string1,
            guid_string2,
            name_guid_string3,
            name_guid_string4,
            name_guid_string5,
        };
        static private string[] elements_par = Array.ConvertAll(elements, delegate(string elem) { return "(" + elem + ")"; });
        static private string guid_string = string.Join("|", elements_par);
        static private Regex reg = new Regex(guid_string);

        private class ProcessGuid
        {
            /// <summary>
            /// GUID format enum
            /// </summary>
            /// <see href="https://msdn.microsoft.com/en-us/library/97af8hh4(v=vs.110).aspx">Guid.ToString Method (String)</see>
            public enum Format
            {
                Unknown,

                /// <summary>
                /// "D": 00000000-0000-0000-0000-000000000000
                /// </summary>
                RawHyphenDigits,

                /// <summary>
                /// "N": 00000000000000000000000000000000
                /// </summary>
                Raw32Digits,

                /// <summary>
                /// "X": {0x00000000, 0x0000, 0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}
                /// </summary>
                GuidVariable,

                /// <summary>
                /// DEFINE_GUID(<<name>>, 0x00000000,0x0000,0x0000, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);
                /// </summary>
                DEFINE_GUID,

                /// <summary>
                /// IMPLEMENT_OLECREATE(<<class>>, <<external_name>>, 0x00000000,0x0000,0x0000, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00);
                /// </summary>
                OLECREATE,
            };

            /// <summary>
            /// delegate for converting a guid to a string
            /// </summary>
            /// <param name="guid"></param>
            /// <returns></returns>
            delegate string ConvertGuid(Guid guid);

            /// <summary>
            /// class for converting GUID
            /// </summary>
            private class MapFormat
            {
                public string Key { get; private set; }
                public Format GuidFormat { get; private set; }
                public ConvertGuid delegateConvertGuid { get; private set; }

                public MapFormat(string Key, Format GuidFormat, ConvertGuid delegateConvertGuid)
                {
                    this.Key = Key;
                    this.GuidFormat = GuidFormat;
                    this.delegateConvertGuid = delegateConvertGuid;
                }
            };

            /// <summary>
            /// 0x00000000,0x0000,0x0000, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
            /// </summary>
            /// <param name="guid"></param>
            /// <returns></returns>
            static string FormatGuidAsRawValues(Guid guid)
            {
                var bytes = guid.ToByteArray();
                var builder = new StringBuilder();
                int i = 0;

                int guid_1st = ((int)bytes[3] << 24) | ((int)bytes[2] << 16) | ((int)bytes[1] << 8) | bytes[0];
                short guid_2nd = (short)(((int)bytes[5] << 8) | bytes[4]);
                short guid_3rd = (short)(((int)bytes[7] << 8) | bytes[6]);

                // 0x00000000
                builder.Append("0x");
                builder.Append(guid_1st.ToString("x8"));
                builder.Append(", ");

                // 0x0000
                builder.Append("0x");
                builder.Append(guid_2nd.ToString("x4"));
                builder.Append(", ");

                // 0x0000
                builder.Append("0x");
                builder.Append(guid_3rd.ToString("x4"));
                builder.Append(", ");

                // 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
                for (i = 0; i < 8; i++)
                {
                    builder.Append("0x");
                    builder.Append(bytes[8+i].ToString("x2"));
                    if (i != 7)
                    {
                        builder.Append(", ");
                    }
                }
                return builder.ToString();
            }

            /// <summary>
            /// GUID formatter table
            /// </summary>
            static private MapFormat[] tableFormats = new MapFormat[] {
                new MapFormat("RawHyphenDigits", Format.RawHyphenDigits, delegate(Guid guid) { return guid.ToString("D"); }),
                new MapFormat("Raw32Digits",     Format.Raw32Digits,     delegate(Guid guid) { return guid.ToString("N"); }),
                new MapFormat("GuidVariable",    Format.GuidVariable,    delegate(Guid guid) { return guid.ToString("X"); }),
                new MapFormat("RAW_GUID_DEF",    Format.DEFINE_GUID,     delegate(Guid guid) { return FormatGuidAsRawValues(guid); }),
                new MapFormat("RAW_GUID_IMP",    Format.OLECREATE,       delegate(Guid guid) { return FormatGuidAsRawValues(guid); }),
            };
 
            private Match m;

            private MapFormat mapFormat;

            /// <summary>
            /// key for Guid dictionary
            /// </summary>
            public string Key { get; private set; }

            /// <summary>
            /// guid format
            /// </summary>
            public Format GuidFormat { get; private set; }

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="m"></param>
            public ProcessGuid(Match m)
            {
                this.m = m;

                this.Key = string.Empty;
                this.mapFormat = null;
                this.GuidFormat = Format.Unknown;
                foreach (MapFormat mapFormat in tableFormats)
                {
                    if (m.Groups[mapFormat.Key].Success)
                    {
                        Guid guid;
                        var value = m.Groups[mapFormat.Key].Value;

                        // try converting GUID.
                        if (!Guid.TryParse(value, out guid))
                        {
                            var builder = new StringBuilder(value);
                            builder.Replace("0x", "");
                            builder.Replace(",", "");
                            builder.Replace(" ", "");
                            value = builder.ToString();
                        }

                        // retry converting GUID.
                        if (Guid.TryParse(value, out guid))
                        {
                            this.Key = guid.ToString("D");
                        }

                        this.mapFormat = mapFormat;
                        this.GuidFormat = mapFormat.GuidFormat;
                        break;
                    }
                }
            }

            /// <summary>
            /// Format the guid in the original format
            /// </summary>
            /// <param name="guid"></param>
            /// <returns></returns>
            public string Convert(Guid guid)
            {
                if (this.mapFormat != null)
                {
                    return this.mapFormat.delegateConvertGuid(guid);
                }

                // return original string
                return this.m.ToString();
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ReplaceWithNewGuid(NewGuid newGuid = null)
        {
            if(newGuid != null)
            {
                this.delegateNewGuid = newGuid;
            }
            else
            {
                this.delegateNewGuid = delegate { return Guid.NewGuid(); };
            }
        }

        /// <summary>
        /// utility function to create GUID.
        /// </summary>
        /// <returns></returns>
        private Guid CallNewGuid()
        {
            return this.delegateNewGuid();
        }

        /// <summary>
        /// delegate for ReplaceNewGuid
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public string delegateReplaceNewGuid(Match m)
        {
            var processGuid = new ProcessGuid(m);
            var newGuid = CallNewGuid();

            var guid_str = processGuid.Convert(newGuid);
            return guid_str;
        }

        /// <summary>
        /// delegate for ReplaceSameGuidToSameGuid
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public string delegateReplaceSameGuidToSameGuid(Match m)
        {
            var processGuid = new ProcessGuid(m);
            var key = processGuid.Key;
            var guid = new Guid(key);
            if (!dict.ContainsKey(key))
            {
                dict[key] = CallNewGuid();
            }
            var newGuid = dict[key];

            var guid_str = processGuid.Convert(newGuid);
            return guid_str;
        }

        /// <summary>
        /// replace GUIDs to new GUIDs.
        /// all GUIDS will be replaced with the different GUIDs.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ReplaceNewGuid(string input)
        {
            //Console.WriteLine(guid_string);
            var myEvaluator = new MatchEvaluator(delegateReplaceNewGuid);

            // Replace matched characters using the delegate method.
            var output = reg.Replace(input, myEvaluator);
            return output;
        }

        /// <summary>
        /// replace GUIDs to new GUIDs.
        /// same GUIDS will be replaced with the same GUIDs.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ReplaceSameGuidToSameGuid(string input)
        {
            //Console.WriteLine(guid_string);
            var myEvaluator = new MatchEvaluator(delegateReplaceSameGuidToSameGuid);

            // Replace matched characters using the delegate method.
            var output = reg.Replace(input, myEvaluator);
            return output;
        }

        public void Dump()
        {
            foreach (KeyValuePair<string, Guid> keyvalue in dict)
            {
                Console.WriteLine("{0}:{1}", keyvalue.Key, keyvalue.Value.ToString());
            }
        }
    }
}
