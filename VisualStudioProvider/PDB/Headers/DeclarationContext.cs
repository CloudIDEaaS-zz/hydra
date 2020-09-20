using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class DeclarationContext : Declaration
    {
        private unsafe CppSharp.Parser.AST.DeclarationContext declarationContext;
        private List<Function> functions;
        private List<Class> classes;
        private List<Namespace> namespaces;
        private List<Enumeration> enums;
        private List<Template> templates;
        private List<TypeDef> typeDefs;
        private List<Variable> variables;
        private List<TypeAlias> typeAliases;
        private List<Friend> friends;
        private string name;

        public DeclarationContext(string name) : base(name)
        {
            this.name = name;
        }

        public unsafe DeclarationContext(CppSharp.Parser.AST.DeclarationContext declarationContext) : base(declarationContext)
        {
            this.declarationContext = declarationContext;
            this.declarationContext.AssertNotNull();
        }

        public unsafe DeclarationContext(Declaration owningDeclaration, CppSharp.Parser.AST.DeclarationContext declarationContext) : base(owningDeclaration, declarationContext)
        {
            this.declarationContext = declarationContext;
            this.declarationContext.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return declarationContext.Location.ID;
            }
        }

        public override string Name
        {
            get
            {
                if (name != null)
                {
                    return name;
                }
                else
                {
                    return base.Name;
                }
            }
        }

        public bool IsAnonymous
        {
            get
            {
                return declarationContext.IsAnonymous;
            }
        }

        public string Access
        {
            get
            {
                return declarationContext.Access.ToString();
            }
        }

        public Comment Comment
        {
            get
            {
                if (declarationContext.Comment == null)
                {
                    return null;
                }
                else
                {
                    return new Comment(this, declarationContext.Comment);
                }
            }
        }

        public string DebugText
        {
            get
            {
                return declarationContext.DebugText;
            }
        }

        public virtual IEnumerable<Function> Functions
        {
            get
            {
                if (functions != null)
                {
                    foreach (var function in functions)
                    {
                        yield return function;
                    }
                }
                else
                {
                    foreach (var function in declarationContext.GetFunctions())
                    {
                        if (function is CppSharp.Parser.AST.Method)
                        {
                            yield return new Method((CppSharp.Parser.AST.Method)function);
                        }
                        else
                        {
                            yield return new Function(this, function);
                        }
                    }
                }
            }
        }

        public virtual IEnumerable<Class> Classes
        {
            get
            {
                if (classes != null)
                {
                    foreach (var _class in classes)
                    {
                        yield return _class;
                    }
                }
                else
                {
                    foreach (var _class in declarationContext.GetClasses())
                    {
                        yield return _class.GetRealClassInternal();
                    }
                }
            }
        }

        public virtual IEnumerable<Namespace> Namespaces
        {
            get
            {
                if (namespaces != null)
                {
                    foreach (var _namespace in namespaces)
                    {
                        yield return _namespace;
                    }
                }
                else
                {
                    foreach (var _namespace in declarationContext.GetNamespaces())
                    {
                        yield return new Namespace(_namespace);
                    }
                }
            }
        }

        public virtual IEnumerable<Enumeration> Enums
        {
            get
            {
                var x = 0;

                if (enums != null)
                {
                    foreach (var _enum in enums)
                    {
                        yield return _enum;
                    }
                }
                else
                {
                    foreach (var enumeration in declarationContext.GetEnums())
                    {
                        yield return new Enumeration(this, enumeration, x++);
                    }
                }
            }
        }

        public virtual IEnumerable<Template> Templates
        {
            get
            {
                if (templates != null)
                {
                    foreach (var template in templates)
                    {
                        yield return template;
                    }
                }
                else
                {
                    foreach (var template in declarationContext.GetTemplates())
                    {
                        yield return template.GetRealTemplateInternal(this);
                    }
                }
            }
        }

        public virtual IEnumerable<TypeDef> TypeDefs
        {
            get
            {
                if (typeDefs != null)
                {
                    foreach (var typeDef in typeDefs)
                    {
                        yield return typeDef;
                    }
                }
                else
                {
                    foreach (var typeDef in declarationContext.GetTypeDefs())
                    {
                        yield return new TypeDef(this, typeDef);
                    }
                }
            }
        }

        public virtual IEnumerable<TypeAlias> TypeAliases
        {
            get
            {
                if (typeAliases != null)
                {
                    foreach (var typeAlias in typeAliases)
                    {
                        yield return typeAlias;
                    }
                }
                else
                {
                    foreach (var typeAlias in declarationContext.GetTypeAliases())
                    {
                        yield return new TypeAlias(this, typeAlias);
                    }
                }
            }
        }

        public virtual IEnumerable<Variable> Variables
        {
            get
            {
                if (variables != null)
                {
                    foreach (var variable in variables)
                    {
                        yield return variable;
                    }
                }
                else
                {
                    foreach (var variable in declarationContext.GetVariables())
                    {
                        if (variable is CppSharp.Parser.AST.VarTemplateSpecialization)
                        {
                            yield return new VarTemplateSpecialization(this, (CppSharp.Parser.AST.VarTemplateSpecialization) variable);
                        }
                        else
                        {
                            yield return new Variable(this, variable);
                        }
                    }
                }
            }
        }

        public virtual IEnumerable<Friend> Friends
        {
            get
            {
                if (friends != null)
                {
                    foreach (var friend in friends)
                    {
                        yield return friend;
                    }
                }
                else
                {
                    foreach (var friend in declarationContext.GetFriends())
                    {
                        yield return new Friend(this, friend);
                    }
                }
            }
        }

        internal int CacheObjects()
        {
            var count = 0;

            functions = this.Functions.ToList();
            classes = this.Classes.ToList();
            namespaces = this.Namespaces.ToList();
            enums = this.Enums.ToList();
            templates = this.Templates.ToList();
            typeDefs = this.TypeDefs.ToList();
            variables = this.Variables.ToList();
            typeAliases = this.TypeAliases.ToList();
            friends = this.Friends.ToList();

            count += functions.Count;
            count += classes.Count;
            count += namespaces.Count;
            count += enums.Count;
            count += templates.Count;
            count += typeDefs.Count;
            count += variables.Count;
            count += typeAliases.Count;
            count += friends.Count;

            return count;
        }
    }
}
