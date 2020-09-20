using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public static class GuidExtensions
    {
        public static string GenerateString(this Guid guid)
        {
            string name;
            LinkedList<char> linkedList;
            LinkedListNode<char> current;
            var currentNumber = string.Empty; ;
            var list = guid.ToString().RemoveText("-").ToCharArray().Select(c =>
            {
                if (c.IsBetween('0', '9'))
                {
                    currentNumber += c.ToString();

                    switch (c)
                    {
                        case '0':
                            c = 'a';
                            break;
                        case '1':
                            c = 'e';
                            break;
                        case '2':
                            c = 'i';
                            break;
                        case '3':
                            c = 'o';
                            break;
                        case '4':
                            c = 'u';
                            break;
                        case '5':
                            c = 'y';
                            break;
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            c = "bcdfghjklmnpqrstvwxyz".ToCharArray().ElementAt(int.Parse(currentNumber.RemoveStartIfMatches("0").Crop(5)).ScopeRange(0, 20));
                            break;
                    }
                }

                return c;
            }).ToList();

            name = string.Join("", list);

            linkedList = new LinkedList<char>(list);

            current = linkedList.First;

            while (current != null)
            {
                if (current.Next == null)
                {
                    break;
                }

                if (current.Value.IsConsonant() && current.Next.Value.IsConsonant())
                {
                    var blendTest = current.Value.ToString() + current.Next.Value.ToString();

                    if (current.Previous != null && current.Previous.Value.IsConsonant())
                    {
                        linkedList.Remove(current.Next);
                        name = string.Join("", linkedList);

                        continue;
                    }

                    switch (blendTest)
                    {
                        case "bl":
                        case "br":
                        case "cl":
                        case "cr":
                        case "dr":
                        case "fr":
                        case "tr":
                        case "fl":
                        case "gl":
                        case "gr":
                        case "pl":
                        case "pr":
                        case "sl":
                        case "sm":
                        case "sp":
                        case "st":
                            break;
                        default:
                            linkedList.Remove(current.Next);
                            name = string.Join("", linkedList);

                            continue;
                    }
                }
                else if (current.Value.IsVowel() && current.Next.Value.IsVowel())
                {
                    var blendTest = current.Value.ToString() + current.Next.Value.ToString();

                    if (current.Previous != null && current.Previous.Value.IsVowel())
                    {
                        linkedList.Remove(current.Next);
                        name = string.Join("", linkedList);

                        continue;
                    }

                    switch (blendTest)
                    {
                        case "ai":
                        case "ie":
                        case "oa":
                        case "ea":
                        case "ue":
                        case "oo":
                        case "ui":
                        case "pr":
                        case "ee":
                            break;
                        default:
                            linkedList.Remove(current.Next);
                            name = string.Join("", linkedList);

                            continue;
                    }
                }

                current = current.Next;
            }

            name = string.Join("", linkedList.Take(10));

            return name;
        }
    }
}
