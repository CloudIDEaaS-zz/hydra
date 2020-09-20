using System;
using System.Collections.Generic;
using System.Text;

namespace VisualStudioProvider
{
    public enum CodePointType
    {
        AfterLastMember,
        AfterLastField,
    }

    public class CodeBlockPoints
    {
        public int Start = -1;
        public int End = -1;

        public CodeBlockPoints(int nStart, int nEnd)
        {
            Start = nStart;
            End = nEnd;
        }

        public bool NotFound
        {
            get
            {
                return Start == -1 && End == -1;
            }
        }
    }

    public class CodePoint
    {
        public static byte[] GetBOM(byte[] rgFileContents)
        {
            // VS 2008 uses a Byte Order Mark (BOM) of 3 header characters

            if (rgFileContents[0] == 0xEF && rgFileContents[1] == 0xBB && rgFileContents[2] == 0xBF)
            {
                byte[] rgBOM = new byte[3];

                rgBOM[0] = rgFileContents[0];
                rgBOM[1] = rgFileContents[1];
                rgBOM[2] = rgFileContents[2];

                return rgBOM;
            }

            return new byte[0];
        }

        //public static int Find(InterfaceNode objInterface, CodePointType eType, TokenCollection rgTokens)
        //{
        //    MemberNode objMember = null;
        //    int nInsert = 0;
        //    LinkedListNode<Token> objNode;

        //    foreach (InterfacePropertyNode objProperty in objInterface.Properties)
        //    {
        //        if (objProperty.RelatedToken.Line > nInsert)
        //        {
        //            objMember = objProperty;
        //            nInsert = objProperty.RelatedToken.Line;
        //        }
        //    }

        //    foreach (InterfaceMethodNode objMethod in objInterface.Methods)
        //    {
        //        if (objMethod.RelatedToken.Line > nInsert)
        //        {
        //            objMember = objMethod;
        //            nInsert = objMethod.RelatedToken.Line;
        //        }
        //    }

        //    if (objMember == null)
        //    {
        //        objNode = rgTokens.Find(objInterface.RelatedToken);

        //        while (objNode.Value.ID != TokenID.RCurly)
        //        {
        //            objNode = objNode.Next;
        //        }

        //        if (objNode.Previous.Value.ID == TokenID.Newline)
        //        {
        //            objNode = objNode.Previous;
        //        }

        //        nInsert = objNode.Value.CharPos;
        //    }
        //    else
        //    {
        //        objNode = rgTokens.Find(objMember.RelatedToken);

        //        while (objNode.Value.ID != TokenID.Semi)
        //        {
        //            objNode = objNode.Next;
        //        }

        //        if (objNode.Next.Value.ID == TokenID.Newline)
        //        {
        //            objNode = objNode.Next;
        //        }

        //        nInsert = objNode.Value.CharPos;
        //    }

        //    return nInsert;
        //}

        //public static int Find(ClassNode objClass, CodePointType eType, TokenCollection rgTokens)
        //{
        //    MemberNode objMember = null;
        //    int nInsert = 0;
        //    LinkedListNode<Token> objNode;
        //    int nCurlyRef;

        //    foreach (PropertyNode objProperty in objClass.Properties)
        //    {
        //        if (objProperty.RelatedToken.Line > nInsert)
        //        {
        //            objMember = objProperty;
        //            nInsert = objProperty.RelatedToken.Line;
        //        }
        //    }

        //    foreach (MethodNode objMethod in objClass.Methods)
        //    {
        //        if (objMethod.RelatedToken.Line > nInsert)
        //        {
        //            objMember = objMethod;
        //            nInsert = objMethod.RelatedToken.Line;
        //        }
        //    }

        //    foreach (ConstructorNode objConstructor in objClass.Constructors)
        //    {
        //        if (objConstructor.RelatedToken.Line > nInsert)
        //        {
        //            objMember = objConstructor;
        //            nInsert = objConstructor.RelatedToken.Line;
        //        }
        //    }

        //    nCurlyRef = -1;

        //    if (objMember == null)
        //    {
        //        objNode = rgTokens.Find(objClass.RelatedToken);

        //        while (objNode != null && nCurlyRef != 0)
        //        {
        //            if (objNode.Value.ID == TokenID.LCurly)
        //            {
        //                if (nCurlyRef == -1)
        //                {
        //                    nCurlyRef = 0;
        //                }

        //                nCurlyRef++;
        //            }
        //            else if (objNode.Value.ID == TokenID.RCurly)
        //            {
        //                nCurlyRef--;
        //            }

        //            if (objNode.Next != null)
        //            {
        //                objNode = objNode.Next;
        //            }
        //        }

        //        while (objNode.Value.ID != TokenID.RCurly)
        //        {
        //            objNode = objNode.Previous;
        //        }

        //        nInsert = objNode.Value.CharPos;
        //    }
        //    else
        //    {
        //        objNode = rgTokens.Find(objMember.RelatedToken);

        //        while (objNode != null && nCurlyRef != 0)
        //        {
        //            if (objNode.Value.ID == TokenID.LCurly)
        //            {
        //                if (nCurlyRef == -1)
        //                {
        //                    nCurlyRef = 0;
        //                }

        //                nCurlyRef++;
        //            }
        //            else if (objNode.Value.ID == TokenID.RCurly)
        //            {
        //                nCurlyRef--;
        //            }

        //            objNode = objNode.Next;
        //        }

        //        nInsert = objNode.Value.CharPos;
        //    }

        //    return nInsert;
        //}

        //public static CodeBlockPoints FindMember(string strMember, InterfaceNode objInterface, TokenCollection rgTokens)
        //{
        //    MemberNode objMember = null;
        //    int nStart = -1;
        //    int nEnd = -1;
        //    LinkedListNode<Token> objNode = null;

        //    foreach (InterfacePropertyNode objProperty in objInterface.Properties)
        //    {
        //        if (objProperty.Names[0].GenericIdentifier == strMember)
        //        {
        //            objMember = objProperty;
        //            objNode = rgTokens.Find(objMember.RelatedToken);

        //            while (objNode.Value.ID != TokenID.Newline)
        //            {
        //                objNode = objNode.Previous;
        //            }

        //            nStart = objNode.Value.CharPos;

        //            break;
        //        }
        //    }

        //    if (objMember == null)
        //    {
        //        foreach (InterfaceMethodNode objMethod in objInterface.Methods)
        //        {
        //            if (objMethod.Names[0].GenericIdentifier == strMember)
        //            {
        //                objMember = objMethod;
        //                objNode = rgTokens.Find(objMember.RelatedToken);

        //                while (objNode.Value.ID != TokenID.Newline)
        //                {
        //                    objNode = objNode.Previous;
        //                }

        //                nStart = objNode.Value.CharPos;

        //                break;
        //            }
        //        }
        //    }

        //    if (objMember != null)
        //    {
        //        while (objNode.Value.ID != TokenID.Semi)
        //        {
        //            objNode = objNode.Next;
        //        }

        //        if (objNode.Next.Value.ID == TokenID.Newline)
        //        {
        //            objNode = objNode.Next;
        //        }

        //        nEnd = objNode.Value.CharPos - 1;
        //    }

        //    return new CodeBlockPoints(nStart, nEnd);
        //}

        //public static CodeBlockPoints FindMember(string strMember, ClassNode objClass, TokenCollection rgTokens)
        //{
        //    MemberNode objMember = null;
        //    int nStart = -1;
        //    int nEnd = -1;
        //    LinkedListNode<Token> objNode = null;
        //    int nCurlyRef;

        //    foreach (PropertyNode objProperty in objClass.Properties)
        //    {
        //        if (objProperty.Names[0].GenericIdentifier == strMember)
        //        {
        //            objMember = objProperty;
        //            objNode = rgTokens.Find(objMember.RelatedToken);

        //            while (objNode.Value.ID != TokenID.Newline)
        //            {
        //                objNode = objNode.Previous;
        //            }

        //            nStart = objNode.Value.CharPos;
        //            break;
        //        }
        //    }

        //    if (objMember == null)
        //    {
        //        foreach (MethodNode objMethod in objClass.Methods)
        //        {
        //            if (objMethod.Names[0].GenericIdentifier == strMember)
        //            {
        //                objMember = objMethod;
        //                objNode = rgTokens.Find(objMember.RelatedToken);

        //                while (objNode.Value.ID != TokenID.Newline)
        //                {
        //                    objNode = objNode.Previous;
        //                }

        //                nStart = objNode.Value.CharPos;
        //                break;
        //            }
        //        }
        //    }

        //    if (objMember != null)
        //    {
        //        nCurlyRef = -1;

        //        while (objNode != null && nCurlyRef != 0)
        //        {
        //            if (objNode.Value.ID == TokenID.LCurly)
        //            {
        //                if (nCurlyRef == -1)
        //                {
        //                    nCurlyRef = 0;
        //                }

        //                nCurlyRef++;
        //            }
        //            else if (objNode.Value.ID == TokenID.RCurly)
        //            {
        //                nCurlyRef--;
        //            }

        //            objNode = objNode.Next;
        //        }

        //        nEnd = objNode.Value.CharPos - 1;
        //    }

        //    return new CodeBlockPoints(nStart, nEnd);
        //}
    }
}
