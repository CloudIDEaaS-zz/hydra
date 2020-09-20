using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System.Diagnostics;
using AbstraX.Contracts;
using AbstraX.TypeMappings;
using ImpromptuInterface;
using AbstraX;
using System.Collections;
using EntityProvider.Web.Entities.DatabaseEntities;

namespace EntityProvider.Web
{
    //  EntitiesRoot
    //    Solution
    //      Project
    //        Model
    //          Namespace
    //            Entity_Container
    //              Entity_Set
    //                EntityType
    //                  NavigationProperty
    //                    EntityType 

    

    [EnableClientAccess()]
    public class EntityProviderService : AbstraXDomainService
    {       
        public EntityProviderService()
        {
        }

        [Invoke]
        public override string GetQueryMethodForID(string id)
        {
            return AbstraXExtensions.GetQueryMethodForID(this, id);
        }

        [Invoke]
        public override string GetRootID()
        {           
            return new EntitiesRoot(this).ID;
        }

        public string GetDescription()
        {
            return null;
        }

        public EntitiesRoot GetRoots()
        {
            // TODO - remove this, for testing

            //var entities = this.GetEntities();

            //foreach (var entity in entities)
            //{
            //}

            return new EntitiesRoot(this);
        }

        [Query]
        public IQueryable<Solution> GetSolutions()
        {
            Debug.WriteLine("Loading solutions");

            var hierarchy = new EntityProviderHierarchy<Solution>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }

        [Query]
        public IQueryable<Project> GetProjects()
        {
            Debug.WriteLine("Loading projects");

            var hierarchy = new EntityProviderHierarchy<Project>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }

        [Query]
        public IQueryable<Model> GetModels()
        {
            Debug.WriteLine("Loading models");

            var hierarchy = new EntityProviderHierarchy<Model>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }

        [Query]
        public IQueryable<DbEntityType> GetDbEntityType()
        {
            Debug.WriteLine("Loading DbEntityType");

            var hierarchy = new EntityProviderHierarchy<DbEntityType>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }

        [Query]
        public IQueryable<DbNavigationProperty> GetDbNavigationProperty()
        {
            Debug.WriteLine("Loading GetDbNavigationProperty");

            var hierarchy = new EntityProviderHierarchy<DbNavigationProperty>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }


        [Query]
        public IQueryable<Entity_Container> GetEntityContainers()
        {
            Debug.WriteLine("Loading containers");

            var hierarchy = new EntityProviderHierarchy<Entity_Container>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }

        [Query]
        public IQueryable<Entity_Set> GetEntitySets()
        {
            Debug.WriteLine("Loading entity sets");

            var hierarchy = new EntityProviderHierarchy<Entity_Set>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }

        [Query]
        public IQueryable<EntityType> GetEntities()
        {
            Debug.WriteLine("Loading entities");

            var hierarchy = new EntityProviderHierarchy<EntityType>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }


        [Query]
        public IQueryable<EntityProperty> GetEntityProperties()
        {
            Debug.WriteLine("Loading entity properties");

            var hierarchy = new EntityProviderHierarchy<EntityProperty>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }

        [Query]
        public IQueryable<NavigationProperty> GetNavigationProperties()
        {
            Debug.WriteLine("Loading navigation properties");

            var hierarchy = new EntityProviderHierarchy<NavigationProperty>(this);
            var results = hierarchy.AsQueryable();

            return results;
        }


        [Query]
        public IQueryable<Solution> GetSolutionsForEntitiesRoot(string parentID)
        {
            Debug.WriteLine("Loading solutions for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<Solution>(this);
            var results = hierarchy.AsQueryable().Where(a => a.ParentID == parentID);

            return results;
        }

        [Query]
        public IQueryable<Project> GetProjectsForSolution(string parentID)
        {
            Debug.WriteLine("Loading projects for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<Project>(this);
            var results = hierarchy.AsQueryable().Where(a => a.ParentID == parentID);

            return results;
        }

        [Query]
        public IQueryable<Model> GetModelsForProject(string parentID)
        {
            Debug.WriteLine("Loading models for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<Model>(this);
            var results = hierarchy.AsQueryable().Where(a => a.ParentID == parentID);

            return results;
        }

        [Query]
        public IQueryable<Entity_Container> GetEntityContainersForModel(string parentID)
        {
            Debug.WriteLine("Loading containers for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<Entity_Container>(this);
            var results = hierarchy.AsQueryable().Where(a => a.ParentID == parentID);

            return results;
        }

        [Query]
        public IQueryable<Entity_Set> GetEntitySetsForEntity_Container(string parentID)
        {
            Debug.WriteLine("Loading entity sets for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<Entity_Set>(this);
            var results = hierarchy.AsQueryable().Where(a => a.ParentID == parentID);

            return results;
        }

        [Query]
        public IQueryable<EntityType> GetEntitiesForEntity_Set(string parentID)
        {
            Debug.WriteLine("Loading entities for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<EntityType>(this);
            var results = hierarchy.AsQueryable().Where(a => a.ParentID == parentID);

            return results;
        }

        [Query, AbstraXProviderMessageInspectorOperationBehaviorAttribute]
        public IQueryable<EntityProperty> GetEntityPropertiesForEntityType(string parentID)
        {
            Debug.WriteLine("Loading entity properties for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<EntityProperty>(this, parentID);
            var results = hierarchy.AsQueryable().Where(t => t.ParentID == parentID);

            return results;
        }

        [Query]
        public IQueryable<NavigationProperty> GetNavigationPropertiesForEntityType(string parentID)
        {
            Debug.WriteLine("Loading navigation properties for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<NavigationProperty>(this, parentID);
            var results = hierarchy.AsQueryable().Where(t => t.ParentID == parentID);

            return results;
        }

        [Query]
        public IQueryable<DbNavigationProperty> GetDbNavigationPropertiesForDbEntityType(string parentID)
        {
            Debug.WriteLine("Loading navigation properties for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<DbNavigationProperty>(this, parentID);
            var results = hierarchy.AsQueryable().Where(t => t.ParentID == parentID);

            return results;
        }

        [Query]
        public IQueryable<EntityType> GetEntitiesForNavigationProperty(string parentID)
        {
            Debug.WriteLine("Loading navigation properties for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<EntityType>(this, parentID);
            var results = hierarchy.AsQueryable().Where(t => t.ParentID == parentID);

            return results;
        }

        [Query]
        public IQueryable<DbEntityType> GetDbEntitiesForDbNavigationProperty(string parentID)
        {
            Debug.WriteLine("Loading navigation properties for '" + parentID + "'");

            var hierarchy = new EntityProviderHierarchy<DbEntityType>(this, parentID);
            var results = hierarchy.AsQueryable().Where(t => t.ParentID == parentID);

            return results;
        }

        [Invoke]
        public override byte[] GetImageForFolder(string folderKey)
        {
            Debug.WriteLine("Loading images for '" + folderKey + "'");

            var stream = this.GetType().Assembly.GetManifestResourceStream(folderKey);
            var bytes = new byte[stream.Length];

            stream.Read(bytes, 0, (int)stream.Length);

            return bytes;
        }

        [Invoke]
        public override byte[] GetImageForItemType(string itemTypeName)
        {
            Debug.WriteLine("Loading images for '" + itemTypeName + "'");

            var attribute = (NodeImageAttribute)System.Type.GetType(itemTypeName).GetCustomAttributes(true).FirstOrDefault(a => a is NodeImageAttribute);

            var stream = this.GetType().Assembly.GetManifestResourceStream(attribute.ImagePath);
            var bytes = new byte[stream.Length];

            stream.Read(bytes, 0, (int)stream.Length);

            return bytes;
        }

        [Invoke]
        public override byte[] GetImageForUrl(string url)
        {
            Debug.WriteLine("Loading images for '" + url + "'");

            var stream = this.GetType().Assembly.GetManifestResourceStream(url);
            var bytes = new byte[stream.Length];

            stream.Read(bytes, 0, (int)stream.Length);

            return bytes;
        }

        public override IBase GenerateByID(string id)
        {
            return AbstraXExtensions.GenerateByID(this, id);
        }

        public override ContainerType GetAllowableContainerTypes(string id)
        {
            throw new NotImplementedException();
        }

        public override ConstructType GetAllowableConstructTypes(string id)
        {
            throw new NotImplementedException();
        }

        public override SortedList<float, IPipelineStep> InitialPipelineSteps
        {
            get
            {
                return null;
            }
        }

        public override void ClearCache()
        {
            EntityProviderHierarchy<IBase>.ClearCache();
        }
       
       [Invoke]
        public  Dictionary<string,string> GetElements()
        {            
            Dictionary<string, string> dbEntities = new Dictionary<string, string>();

            var list2 = this.DomainServiceHostApplication.DatabaseProxy.GetAllElements().Where(p=>p.RelatedID==null && p.ParentID.Contains("@DbEntitySet=")).OrderBy(p=>p.Name);
            
            foreach (var element in list2)
            {
                dbEntities.Add(element.DatabaseID, element.Name);
            }           

            return dbEntities;
        }

        public string GetEntityDatabaseID(string ID, string predicate)
        {
            Guid result;
            if (Guid.TryParse(ID, out result))
            {
                return ID;
            }
            else
            {
                string DbID;
                DbID = ID.Split('@').ToList().Last(p => p.StartsWith(predicate)).ToString();
                DbID = System.Text.RegularExpressions.Regex.Match(DbID, "[a-fA-F0-9]{8}-([a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}").Value;
                return DbID;
            }
        }

        private IDbElement MakeEntitySetElement(string parentID, string nodeText)
        {
            var element = new
            {
                // list all fields and values

                Name = nodeText,
                ParentID = parentID,
                FolderKeyPair = "",
                DebugInfo = "",
                ChildOrdinal = 0,
                DesignComments = "",
                Documentation = "",
                ImageURL = "",
                DataType = string.Empty,
                RelatedID = "",
                Parent = GenerateByID(parentID)
            }.ActLike<IDbElement>();

            return element;
        }

        private IDbElement MakeEntityTypeElement(string parentID, string nodeText)
        {
            var element = new
            {
                // list all fields and values
                Name = nodeText,
                ParentID = parentID,
                FolderKeyPair = "",
                DebugInfo = "",
                ChildOrdinal = 0,
                DesignComments = "",
                Documentation = "",
                ImageURL = "",
                DataType = string.Empty,
                RelatedID = ""
            }.ActLike<IDbElement>();

            return element;
        }

        private IDbElement MakeNavEntityType(string setNodeText, string parentID, string ID)
        {
            var elementnav = new
            {
                // list all fields and values

                Name = setNodeText,
                ParentID = parentID,
                FolderKeyPair = "",
                DebugInfo = "",
                ChildOrdinal = 0,
                DesignComments = "",
                Documentation = "",
                ImageURL = "",
                DataType = string.Empty,
                RelatedID = ID
            }.ActLike<IDbElement>();

            return elementnav;
        }

        private IAttribute MakePropertyAttribute(string parentID, string nodeText)
        {
            var attribute = new
            {
                // list all fields and values

                Name = nodeText,
                ParentID = parentID,
                FolderKeyPair = "",
                DebugInfo = "",
                ChildOrdinal = 0,
                DesignComments = "",
                Documentation = "",
                ImageURL = "",
            }.ActLike<IAttribute>();

            return attribute;
        }

        #region Custom Service Methods

        #region EntityContainer

        [Invoke]
        public string AddNewEntityContainer(string parentObjectID, string nodeText)
        {
            return Guid.NewGuid().ToString();
        }

        [Invoke]
        public string RenameEntityContainer(string parentID, string nodeText)
        {
            return Guid.NewGuid().ToString();
        }

        [Invoke]
        public string DeleteEntityContainer(string parentID)
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

        #region EntitySet

        [Invoke]
        public string AddNewEntitySet(string parentID, string nodeText)
        {
         
            string setNodeText = DetermineGrammer(nodeText, "EntitySet");
            string ID = this.DomainServiceHostApplication.DatabaseProxy.AddNewElement(MakeEntitySetElement(parentID, setNodeText));
            string entitySetId = AbstraXExtensions.MakeID(MakeEntitySetElement(parentID, setNodeText), "DbEntitySet", "DbEntitySet='" + ID + "'");
            AddNewEntityType(entitySetId, nodeText);

            return ID;
        }


        // Shenkey TODO - Update RenameEntitySet Method
        [Invoke]
        public void RenameEntitySet(string parentID, string nodeText, string oldName)
        {
            
            string setNodeText = DetermineGrammer(nodeText, "EntitySet");
            string entityId = this.DomainServiceHostApplication.DatabaseProxy.GetAllElements().Where(p => p.ParentID == parentID).Select(e => e.DatabaseID ).SingleOrDefault().ToString();
            this.DomainServiceHostApplication.DatabaseProxy.UpdateElement(GetEntityDatabaseID(parentID, "DbEntitySet="), setNodeText);
            this.DomainServiceHostApplication.DatabaseProxy.UpdateElement(entityId , DetermineGrammer(nodeText ,"EntityType"));
            

        }

        [Invoke]
        public void DeleteEntitySet(string parentID, string nodeText)
        {

            var element = new
            {
                // list all fields and values
                Name = nodeText,
                ParentID = parentID,
                Parent = GenerateByID(parentID)

            }.ActLike<IElement>();

            this.DomainServiceHostApplication.DatabaseProxy.DeleteElement(element);
            string entitySetId = AbstraXExtensions.MakeID(element, "DbEntitySet", "EntitySet='" + nodeText + "'");
            this.DeleteEntityType(entitySetId, DetermineGrammer(nodeText, "EntityType"));

        }

        #endregion

        #region EntityType

        [Invoke]
        public string AddNewEntityType(string parentID, string nodeText)
        {
            string setNodeText = DetermineGrammer(nodeText, "EntityType");
            string ID = this.DomainServiceHostApplication.DatabaseProxy.AddNewElement(MakeEntityTypeElement(parentID, setNodeText));

            return ID;
        }

        [Invoke]
        public string[] RenameEntityType(string parentID, string newID, string nodeText, string oldName)
        {
            string[] renameEntitytype = new string[2];
            renameEntitytype[0] = DetermineGrammer(nodeText, "EntityType");
            renameEntitytype[1] = DetermineGrammer(nodeText, "EntitySet");
            this.DomainServiceHostApplication.DatabaseProxy.UpdateElement(GetEntityDatabaseID(parentID, "DbEntitySet="), renameEntitytype[1]);
            this.DomainServiceHostApplication.DatabaseProxy.UpdateElement(GetEntityDatabaseID(parentID, "DbEntity="), renameEntitytype[0]);
            //this.DomainServiceHostApplication.DatabaseProxy.UpdateElement(MakeEntityTypeElement(newID, renameEntitytype[1]), DetermineGrammer(oldName, "EntitySet"));
            //this.DomainServiceHostApplication.DatabaseProxy.UpdateElement(MakeEntityTypeElement(parentID, renameEntitytype[0]), oldName);            
            GetEntitySetsForEntity_Container(newID);
            return renameEntitytype;
        }

        [Invoke]
        public void DeleteEntityType(string parentID, string nodeText)
        {           
            string setNodeText = DetermineGrammer(nodeText, "EntityType");
            var element = new
            {
                // list all fields and values
                Name = setNodeText,
                ParentID = parentID,

            }.ActLike<IElement>();

            this.DomainServiceHostApplication.DatabaseProxy.DeleteElement(element);
        }

        [Invoke]
        public string AddNewRelatedEntityType(string parentID)
        {
            return Guid.NewGuid().ToString();
        }

        [Invoke]
        public string AddExistingRelatedEntityType(string parentID)
        {
            return Guid.NewGuid().ToString();
        }

        [Invoke]
        public string RemoveRelatedEntityType(string parentID)
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

        #region Property

        [Invoke]
        public string AddNewProperty(string parentID, string nodeText)
        {
            string ID = this.DomainServiceHostApplication.DatabaseProxy.AddNewAttribute(MakePropertyAttribute(parentID, nodeText));
            return ID;
        }

        [Invoke]
        public string RenameProperty(string parentID, string nodeText, string oldName)
        {
            this.DomainServiceHostApplication.DatabaseProxy.UpdateAttribute(MakePropertyAttribute(parentID, nodeText), oldName);
            return nodeText;
        }

        [Invoke]
        public void DeleteProperty(string parentID, string nodeText)
        {
            var attribute = new
            {
                // list all fields and values
                Name = nodeText,
                ParentID = parentID,

            }.ActLike<IAttribute>();

            this.DomainServiceHostApplication.DatabaseProxy.DeleteAttribute(attribute);
        }

        [Invoke]
        public string ExpandProperty(string parentID, string dataType, string allowNull, string defaultValue, string dataSize, string keyStatus)
        {
            return "Success";
        }

        #endregion

        #region NavigationProperty
        int GetReverseMultiplicity(int multiplicity)
        {
            int flag = 9;
            switch (multiplicity)
            {
                case 1:
                    flag = 1;
                    break;
                case 2:
                    flag = 4;
                    break;
                case 3:
                    flag = 7;
                    break;
                case 4:
                    flag = 2;
                    break;
                case 5:
                    flag = 5;
                    break;
                case 6:
                    flag = 8;
                    break;
                case 7:
                    flag = 3;
                    break;
                case 8:
                    flag = 6;
                    break;
                case 9:
                    flag = 9;
                    break;

            }
            return flag;
        }

        [Invoke]
        public string[] AddNewNavigationProperty(string parentID, string containerID, string nodeText, string navText, int multiplicity)
        {
            string[] IDs = new string[6];
           
            string setNodeText = DetermineGrammer(nodeText, "EntitySet");
            // New Node for navigation property
            IDs[0] = this.DomainServiceHostApplication.DatabaseProxy.AddNewElement(MakeEntitySetElement(containerID, setNodeText));
            string entitySetId = AbstraXExtensions.MakeID(MakeEntitySetElement(containerID, setNodeText), "DbEntitySet", "DbEntitySet='" + IDs[0] + "'");
            IDs[1] = AddNewEntityType(entitySetId, nodeText);
            // create parent id for reverse connection to existing enitytype
            entitySetId = entitySetId + "/DbEntityType[@DbEntity='" + IDs[1] + "']";
            //add new navigation property to node that actully called to create navigation propety
            IDs[2] = this.DomainServiceHostApplication.DatabaseProxy.AddNewElement(MakeNavEntityType(setNodeText, parentID, IDs[1]));
            // add navigation property to newly created node so that reverse connection is possible
            IDs[3] = this.DomainServiceHostApplication.DatabaseProxy.AddNewElement(MakeNavEntityType(DetermineGrammer(navText, "EntitySet"), entitySetId, GetEntityDatabaseID(parentID, "DbEntity=")));
            IDs[4] = SelectMultiplicity(IDs[2], multiplicity);
            IDs[5] = SelectMultiplicity(IDs[3], GetReverseMultiplicity(multiplicity));

            return IDs;
        }       

        [Invoke]
        public void RenameNavigationProperty(string parentID, string nodeText)
        {
            this.DomainServiceHostApplication.DatabaseProxy.UpdateElement(GetEntityDatabaseID(parentID, "DbProperty="), nodeText);
        }

        [Invoke]
        public string DeleteNavigationProperty(string parentID)
        {
            return Guid.NewGuid().ToString();
        }

        [Invoke]
        public string SelectMultiplicity(string parentID, int multiplicity)
        {
            string ID = this.DomainServiceHostApplication.DatabaseProxy.AddNewCustomFields(GetEntityDatabaseID(parentID, "DbProperty="), "Multiplicity", multiplicity.ToString());
            return ID;
        }

        [Invoke]
        public string[] ChangeMultiplicity(string id, string parentID, int multiplicity)
        {
            string[] ID = new string[2];
            ID[0] = GetEntityDatabaseID(id, "DbProperty=");
            ID[1] = GetEntityDatabaseID(parentID, "DbEntity=");
            string relatedID = this.GetRelatedNodeID(ID[1]);
            if (relatedID != string.Empty)
            {
                ID[0] = SelectMultiplicity(ID[0], multiplicity);
                ID[1] = SelectMultiplicity(relatedID, GetReverseMultiplicity(multiplicity));
            }

            return ID; 
        }

        public string GetRelatedNodeID(string id)
        {
            string nodeId= string.Empty;
            var elements = this.DomainServiceHostApplication.DatabaseProxy.GetAllElements().Where(p=>p.RelatedID == id);
            foreach (var element in elements)
            {
                nodeId = element.ID.ToString();
            }

            return nodeId;
        }
        [Invoke]
        public string[] ExpandNavigationPropertyExisting(string parentID, string containerId,string nodeText, string navText, string relatedObjectId, int multiplicity)
        {
            string[] IDs = new string[4];

            string setNodeText = DetermineGrammer(nodeText, "EntitySet");
            
            string entitySetId  = this.DomainServiceHostApplication.DatabaseProxy.GetAllElements().Where(p => p.DatabaseID == relatedObjectId).Select(e => e.ParentID).SingleOrDefault().ToString();           
            // create parent id for reverse connection to existing enitytype
            entitySetId = entitySetId + "/DbEntityType[@DbEntity='" + relatedObjectId + "']";
            //add new navigation property to node that actully called to create navigation propety
            IDs[0] = this.DomainServiceHostApplication.DatabaseProxy.AddNewElement(MakeNavEntityType(DetermineGrammer(navText, "EntitySet"), parentID, relatedObjectId));
            // add navigation property to newly created node so that reverse connection is possible
            IDs[1] = this.DomainServiceHostApplication.DatabaseProxy.AddNewElement(MakeNavEntityType(setNodeText, entitySetId, GetEntityDatabaseID(parentID, "DbEntity=")));
            IDs[2] = SelectMultiplicity(IDs[0], multiplicity);
            IDs[3] = SelectMultiplicity(IDs[1], GetReverseMultiplicity(multiplicity));

            return IDs;
        }

        [Invoke]
        public string ExpandNavigationPropertyNew(string parentID, string newObjectText)
        {
            return "Success";
        }

        #endregion

        #region Comments

/*
        public IQueryable<DbEntityProperty> viewcomments(string id, string type)
        {
            //string[] documentsDetails = new string[2];
            //string attributeDocID;
            //string elementDocID;
           

            switch (type)
            {
                case "EntityProperty":
                    {
                      //check for document id in abstraxattribute table to see whether comment is there or not
                     //   var checkID = GetEntityDatabaseID(id, "DbProperty=");
                    //   var attributeDocID =
                           return this.DomainServiceHostApplication.DatabaseProxy.GetAllAttributes().Where(p => p.ParentID == id).Select(e => e.DocumentationID);

                        //if (attributeDocID != null)
                        //{
                        // var result = this.DomainServiceHostApplication.DatabaseProxy.GetAttibuteComments(GetEntityDatabaseID(id, "DbProperty="));

                        // return <DbEntityProperty>result;
                        //}
                        break;
                    }

                case "EntityType":
                    {
                        //check for dcoument id in abstraxelement table to see whether comment is added or not
                        var elementDocID = this.DomainServiceHostApplication.DatabaseProxy.GetAllElements().Where(p => p.ParentID == id).Select(e => e.DocumentationID);

                        //if (elementDocID != null)
                        //{
                        //   var result = this.DomainServiceHostApplication.DatabaseProxy.GetElementComments(GetEntityDatabaseID(id, "DbEntity="));
                        //}
                        break;
                    }
                     default:
                   var result = null;
                    break;

            }
           
            return result;
        
        }
        */

        [Invoke]
        public  string[] ViewComments(string id, string type)
        {
            
            

            string[] documentsDetails = new string[2];
            string attributeDocID;
            string elementDocID;
      

            switch (type)
            {

/*
                case "Entity_Set":
                    {
                        //check for dcoument id in abstraxelement table to see whether comment is added or not
                        elementDocID = this.DomainServiceHostApplication.DatabaseProxy.checkElementDocIDforAddComment(GetEntityDatabaseID(id, "DbEntitySet="));

                        if (elementDocID != null)
                        {
                            documentsDetails = this.DomainServiceHostApplication.DatabaseProxy.GetElementComments(GetEntityDatabaseID(id, "DbEntitySet="));
                        }
                        break;
                    }
                    */

                case "EntityType":
                    {
                        //check for dcoument id in abstraxelement table to see whether comment is added or not
                        elementDocID = this.DomainServiceHostApplication.DatabaseProxy.checkElementDocIDforAddComment(GetEntityDatabaseID(id, "DbEntity="));

                        if (elementDocID != null)
                        {
                            documentsDetails = this.DomainServiceHostApplication.DatabaseProxy.GetElementComments(GetEntityDatabaseID(id, "DbEntity="));
                        }
                        break;
                    }


                case "EntityProperty":
                    {
                         //check for document id in abstraxattribute table to see whether comment is there or not
                        attributeDocID = this.DomainServiceHostApplication.DatabaseProxy.checkAttributeDocIDforAddComment(GetEntityDatabaseID(id, "DbProperty="));

                        if (attributeDocID != null)
                        {
                            documentsDetails = this.DomainServiceHostApplication.DatabaseProxy.GetAttibuteComments(GetEntityDatabaseID(id, "DbProperty="));

                        }
                        break;
                    }

                
                default:
                    documentsDetails = null;
                    break;

            }
           
            return documentsDetails;
        }
        
        [Invoke]
        public void AddComments(string type, string id, string documentationSummary, string documentation)
        {
            string docID;
            string attributeDocID;
            string elementDocID;
            
            switch (type)
            {


/*
                case "Entity_Set":
                    {
                        //check for dcoument id in abstraxelement table to see whether comment is added or not
                        elementDocID = this.DomainServiceHostApplication.DatabaseProxy.checkElementDocIDforAddComment(GetEntityDatabaseID(id, "DbEntitySet="));

                        if (elementDocID != null)
                        {
                            docID = this.DomainServiceHostApplication.DatabaseProxy.AddNewComments(documentationSummary, documentation);
                            // insert docid in abstraxelement table
                            this.DomainServiceHostApplication.DatabaseProxy.AddElementDocumentaionID(docID, GetEntityDatabaseID(id, "DbEntitySet="));

                        }
                        break;
                        //else
                        //{
                        //    docID = null;
                        //    break;
                        //}
                    }
                */

                case "EntityType":
                    {
                        //check for dcoument id in abstraxelement table to see whether comment is added or not
                        elementDocID = this.DomainServiceHostApplication.DatabaseProxy.checkElementDocIDforAddComment(GetEntityDatabaseID(id, "DbEntity="));

                        if (elementDocID != null)
                        {
                            docID = this.DomainServiceHostApplication.DatabaseProxy.AddNewComments(documentationSummary, documentation);
                            // insert docid in abstraxelement table
                            this.DomainServiceHostApplication.DatabaseProxy.AddElementDocumentaionID(docID, GetEntityDatabaseID(id, "DbEntity="));

                        }
                        break;
                        //else
                        //{
                        //    docID = null;
                        //    break;
                        //}
                    }


                case "EntityProperty":
                    {
                        //check for dcoument id in abstraxattribute table to see whether comment is added or not
                        attributeDocID = this.DomainServiceHostApplication.DatabaseProxy.checkAttributeDocIDforAddComment(GetEntityDatabaseID(id, "DbProperty="));
                       
                        if (attributeDocID != null)
                        {
                            docID = this.DomainServiceHostApplication.DatabaseProxy.AddNewComments(documentationSummary, documentation);
                            // insert docid in abstraxattribute table
                            this.DomainServiceHostApplication.DatabaseProxy.AddAttributeDocumentaionID(docID, GetEntityDatabaseID(id, "DbProperty="));
                         }
                        break;
                        //else
                        //{
                        //    docID = null;
                        //    break;
                        //}
                    }



                
                default:
                    //docID = null;
                    break;

            }
            //return docID;
        }

        [Invoke]
        public string[] EditComments(string id, string type)
        {
            string attributeDocID;
            string elementDocID;
           // string docID;
            string[] documentDetails = new string[2]; 
            try
            {
                switch (type)
                {

         /*           case "Entity_Set":
                        {

                            elementDocID = this.DomainServiceHostApplication.DatabaseProxy.checkElementDocIDforAddComment(GetEntityDatabaseID(id, "DBEntitySet="));

                            if (elementDocID != null)
                            {
                                documentDetails = this.DomainServiceHostApplication.DatabaseProxy.GetElementComments(GetEntityDatabaseID(id, "DbEntitySet="));

                            }
                            break;
                        }*/

                    case "EntityType":
                        {

                            elementDocID = this.DomainServiceHostApplication.DatabaseProxy.checkElementDocIDforAddComment(GetEntityDatabaseID(id, "DBEntity="));

                            if (elementDocID != null)
                            {
                                documentDetails = this.DomainServiceHostApplication.DatabaseProxy.GetElementComments(GetEntityDatabaseID(id, "DbEntity="));

                            }
                            break;
                        }
                    case "EntityProperty":
                        {

                            attributeDocID = this.DomainServiceHostApplication.DatabaseProxy.checkAttributeDocIDforAddComment(GetEntityDatabaseID(id, "DbProperty="));
                            
                            if (attributeDocID != null)
                            {
                                documentDetails = this.DomainServiceHostApplication.DatabaseProxy.GetAttibuteComments(GetEntityDatabaseID(id, "DbProperty="));
                                                              
                            }
                            break;
                        }

                   

                    default:
                      documentDetails = null;
                        break;
                }
                
            }
            catch (System.Data.DataException ex)
            {
                throw ex;
            }
            return documentDetails;
        }


        [Invoke]
        public void checkForCommentstoDelete(string parentID, string type)
        {

            //string docID;
            //string id;
            try
            {
                switch (type)
                {
/*
                    case "Entity_Set":
                        {
                            // docID =
                            this.DomainServiceHostApplication.DatabaseProxy.GetElementCommentDocIDtoDelete(GetEntityDatabaseID(parentID, "DbEntitySet="));
                            break;
                        }*/

                    case "EntityType":
                        {
                            // docID =
                            this.DomainServiceHostApplication.DatabaseProxy.GetElementCommentDocIDtoDelete(GetEntityDatabaseID(parentID, "DbEntity="));
                            break;
                        }
                    case "EntityProperty":
                        {
                           // docID =
                            this.DomainServiceHostApplication.DatabaseProxy.GetAttibuteCommentDocIDtoDelete(GetEntityDatabaseID(parentID, "DbProperty="));


                            break;
                        }
  
                    default:
                       // docID = null;
                        break;

                }

                //if (docID != null)
                //{
                //    this.DomainServiceHostApplication.DatabaseProxy.DeleteComments(docID);
                //}
            }
            catch (System.Data.DataException ex)
            {
                throw ex;
            }
        }
       

        #endregion

        public string DetermineGrammer(string nodeText, string nodeType)
        {
            string setNodeText = string.Empty;

            if (nodeType == "EntitySet")
            {

                if (nodeText.IsSingular())
                {
                    setNodeText = nodeText.Pluralize();
                }
                else
                {
                    setNodeText = nodeText;
                }
            }
            else if (nodeType == "EntityType")
            {
                if (nodeText.IsSingular())
                {
                    setNodeText = nodeText;
                }
                else
                {
                    setNodeText = nodeText.Singularize();
                }
            }

            return setNodeText;
        }

        #endregion
    }
}


