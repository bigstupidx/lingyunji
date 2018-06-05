/*
 * UnityScript Lightweight XML Parser
 * by Fraser McCormick (unityscripts@roguishness.com)
 * http://twitter.com/flimgoblin
 * http://www.roguishness.com/unity/
 *
 * You may use this script under the terms of either the MIT License 
 * or the Gnu Lesser General Public License (LGPL) Version 3. 
 * See:
 * http://www.roguishness.com/unity/lgpl-3.0-standalone.html
 * http://www.roguishness.com/unity/gpl-3.0-standalone.html
 * or
 * http://www.roguishness.com/unity/MIT-license.txt
 */


using System.Collections.Generic;


/* Usage:
 * parser=new XMLParser();
 * var node=parser.Parse("<example><value type=\"String\">Foobar</value><value type=\"Int\">3</value></example>");
 * 
 * Nodes are Boo.Lang.Hash values with text content in "_text" field, other attributes
 * in "@attribute" and any child nodes listed in an array of their nodename.
 * 
 * any XML meta tags <? .. ?> are ignored as are comments <!-- ... -->
 * any CDATA is bundled into the "_text" attribute of its containing node.
 *
 * e.g. the above XML is parsed to:
 * node={ "example": 
 *			[ 
 *			   { "_text":"", 
 *				  "value": [ { "_text":"Foobar", "@type":"String"}, {"_text":"3", "@type":"Int"}]
 *			   } 
 *			],
 *		  "_text":""
 *     }
 *		  
 */
namespace TinyXML
{
    public class XMLParser
    {
        const char LT = '<';
        const char GT = '>';
        const char SPACE = ' ';
        const char QUOTE = '"';
        const char SLASH = '/';
        const char QMARK = '?';
        const char EQUALS = '=';
        const char EXCLAMATION = '!';
        const char DASH = '-';
        const char SQR = ']';

        static bool isContain(System.Func<int, char> content, int start, string s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (content(start + i) != s[i])
                    return false;
            }

            return true;
        }

        static void AddNodeText(System.Text.StringBuilder textValue, XMLNode currentNode)
        {
            if (textValue.Length > 0)
            {
                bool issave = false;
                for (int m = 0; m < textValue.Length; ++m)
                {
                    if (textValue[m] == '\n' || textValue[m] == '\r' || textValue[m] == ' ' || textValue[m] == '\t')
                    {

                    }
                    else
                    {
                        issave = true;
                        break;
                    }
                }

                if (issave)
                {
                    currentNode.text += textValue.ToString();
                }
            }

            textValue.Length = 0;
        }

        public static XMLNode Parse(System.Func<int, char> content, int lenght)
        {
            XMLNode rootNode = XMLNode.Get();

            bool inElement = false;
            bool collectNodeName = false;
            bool collectAttributeName = false;
            bool collectAttributeValue = false;
            bool quoted = false;
            System.Text.StringBuilder attName = new System.Text.StringBuilder();
            System.Text.StringBuilder attValue = new System.Text.StringBuilder();
            System.Text.StringBuilder nodeName = new System.Text.StringBuilder();
            System.Text.StringBuilder textValue = new System.Text.StringBuilder();

            bool inMetaTag = false;
            bool inComment = false;
            bool inDoctype = false;
            bool inCDATA = false;

            XMLNode currentNode = rootNode;
            for (int i = 0; i < lenght; i++)
            {
                char c = content(i);
                char cn = '\0';
                char cnn = '\0';
                char cp = '\0';
                if ((i + 1) < lenght)
                    cn = content(i + 1);

                if ((i + 2) < lenght)
                    cnn = content(i + 2);

                if (i > 0)
                    cp = content(i - 1);

                if (inMetaTag)
                {
                    if (c == QMARK && cn == GT)
                    {
                        inMetaTag = false;
                        i++;
                    }
                    continue;
                }
                else
                {
                    if (!quoted && c == LT && cn == QMARK)
                    {
                        inMetaTag = true;
                        continue;
                    }
                }

                if (inDoctype)
                {
                    if (cn == GT)
                    {
                        inDoctype = false;
                        i++;
                    }
                    continue;
                }
                else if (inComment)
                {
                    if (cp == DASH && c == DASH && cn == GT)
                    {
                        inComment = false;
                        i++;
                    }
                    continue;
                }
                else
                {
                    if (!quoted && c == LT && cn == EXCLAMATION)
                    {
                        if (lenght > i + 2 && isContain(content, i, "<!--"))
                        {
                            inComment = true;
                            i += 3;
                        }
                        else if (lenght > i + 9 && isContain(content, i, "<![CDATA["))
                        {
                            inCDATA = true;
                            i += 8;
                        }
                        else if (lenght > i + 9 && isContain(content, i, "<!DOCTYPE"))
                        {
                            inDoctype = true;
                            i += 8;
                        }

                        continue;
                    }
                }

                if (inCDATA)
                {
                    if (c == SQR && cn == SQR && cnn == GT)
                    {
                        inCDATA = false;
                        i += 2;
                        continue;
                    }

                    textValue.Append(c);
                    continue;
                }


                if (inElement)
                {
                    if (collectNodeName)
                    {
                        if (c == SPACE)
                        {
                            collectNodeName = false;
                        }
                        else if (c == GT)
                        {
                            collectNodeName = false;
                            inElement = false;
                        }

                        if (!collectNodeName && nodeName.Length > 0)
                        {
                            if (nodeName[0] == SLASH)
                            {
                                // close tag
                                AddNodeText(textValue, currentNode);

                                nodeName.Length = 0;
                                currentNode = currentNode.parent;
                            }
                            else
                            {

                                AddNodeText(textValue, currentNode);

                                XMLNode newNode = XMLNode.Get();
                                newNode.text = "";
                                newNode.name = nodeName.ToString();
                                currentNode.childList.Add(newNode);
                                newNode.parent = currentNode;

                                currentNode = newNode;
                                nodeName.Length = 0;
                            }
                        }
                        else
                        {
                            nodeName.Append(c);
                        }
                    }
                    else
                    {

                        if (!quoted && c == SLASH && cn == GT)
                        {
                            inElement = false;
                            collectAttributeName = false;
                            collectAttributeValue = false;
                            if (attName.Length != 0)
                                currentNode.attributes[attName.ToString()] = attValue.ToString();

                            i++;
                            currentNode = currentNode.parent;
                            attName.Length = 0;
                            attValue.Length = 0;
                        }
                        else if (!quoted && c == GT)
                        {
                            inElement = false;
                            collectAttributeName = false;
                            collectAttributeValue = false;
                            if (attName.Length != 0)
                                currentNode.attributes[attName.ToString()] = attValue.ToString();

                            attName.Length = 0;
                            attValue.Length = 0;
                        }
                        else
                        {
                            if (collectAttributeName)
                            {
                                if (c == SPACE || c == EQUALS)
                                {
                                    collectAttributeName = false;
                                    collectAttributeValue = true;
                                }
                                else
                                {
                                    attName.Append(c);
                                }
                            }
                            else if (collectAttributeValue)
                            {
                                if (c == QUOTE)
                                {
                                    if (quoted)
                                    {
                                        collectAttributeValue = false;
                                        if (attName.Length != 0)
                                            currentNode.attributes[attName.ToString()] = attValue.ToString();

                                        attName.Length = 0;
                                        attValue.Length = 0;
                                        quoted = false;
                                    }
                                    else
                                    {
                                        quoted = true;
                                    }
                                }
                                else
                                {
                                    if (quoted)
                                    {
                                        attValue.Append(c);
                                    }
                                    else
                                    {
                                        if (c == SPACE)
                                        {
                                            collectAttributeValue = false;
                                            if (attName.Length != 0)
                                                currentNode.attributes[attName.ToString()] = attValue.ToString();

                                            attName.Length = 0;
                                            attValue.Length = 0;
                                        }
                                    }
                                }
                            }
                            else if (c == SPACE)
                            {

                            }
                            else
                            {
                                collectAttributeName = true;
                                attName.Length = 0;
                                attName.Append(c);
                                attValue.Length = 0;
                                quoted = false;
                            }
                        }
                    }
                }
                else
                {
                    if (c == LT)
                    {
                        inElement = true;
                        collectNodeName = true;
                    }
                    else
                    {
                        textValue.Append(c);
                    }

                }

            }

            return rootNode;
        }

    }
}
