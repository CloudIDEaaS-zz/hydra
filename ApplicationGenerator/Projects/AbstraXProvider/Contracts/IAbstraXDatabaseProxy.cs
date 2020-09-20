using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.ServerInterfaces;

namespace AbstraX.Contracts
{
    public interface IAbstraXDatabaseProxy
    {
        List<IDbElement> GetChildElements(string parentID);
        List<IDbAttribute> GetAttributes(string parentID);
        List<IDbOperation> GetOperations(string parentID);
        //List<IDbElement> GetChildOfNavigationEntities(string relatedID);
        List<Facet> GetFacets(string parentID);
        BaseType GetBaseType(string parentID);
        ICustomFields GetCustomFields(string parentID);
        string AddNewAttribute(IAttribute attribute);
        void UpdateAttribute(IAttribute attribute, string oldName);
        void DeleteAttribute(IAttribute attribute);
        string AddNewElement(IDbElement element);
        //string AddNewNavElement(IDbElement element);
        void UpdateElement(string entityID, string newName);
        void DeleteElement(IElement element);
        //void UpdateNavigationProperty(string ID, string nodeText);
        string AddNewCustomFields(string parentID, string name, string fieldValue);        
        //string GetRelatedNodeID(string ID);        
        List<IDbElement>GetAllElements();       
        string AddNewComments(string documentationSummary, string documentation);
        string checkAttributeDocIDforAddComment(string id);
        void AddAttributeDocumentaionID(string docID, string Id);
        string checkElementDocIDforAddComment(string id);
        void AddElementDocumentaionID(string docID, string Id);
        string[] GetElementComments(string ID);
        string[] GetAttibuteComments(string ID);
        void GetAttibuteCommentDocIDtoDelete(string ID);
        void GetElementCommentDocIDtoDelete(string ID);
        void UpdateComments(string id, string documentationSummary, string documentation);        
        List<IDbAttribute> GetAllAttributes();
        
    }
}
