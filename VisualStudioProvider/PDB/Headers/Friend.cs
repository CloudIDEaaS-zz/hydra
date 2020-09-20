using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Friend : Declaration
    {
        private unsafe CppSharp.Parser.AST.Friend friend;

        public unsafe Friend(CppSharp.Parser.AST.Friend friend) : base(friend)
        {
            this.friend = friend;
            this.friend.AssertNotNullAndOfType<CppSharp.Parser.AST.Friend>();
        }

        public unsafe Friend(Declaration owningDeclaration, CppSharp.Parser.AST.Friend friend) : base(owningDeclaration, friend)
        {
            this.friend = friend;
            this.friend.AssertNotNullAndOfType<CppSharp.Parser.AST.Friend>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return friend.Location.ID;
            }
        }

        public Declaration FriendDeclaration
        {
            get
            {
                if (friend.Declaration == null)
                {
                    return null;
                }
                else
                {
                    return friend.Declaration.GetRealDeclarationInternal();
                }
            }
        }
    }
}
