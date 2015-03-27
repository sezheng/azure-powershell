// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Azure.Management.Resources.Models;
using AuthorizationResourceIdentity = Microsoft.Azure.ResourceIdentity;
using ProjectResources = Microsoft.Azure.Commands.Resources.Properties.Resources;
using ResourcesResourceIdentity = Microsoft.Azure.ResourceIdentity;
//using Microsoft.Azure.Commands.Resources.Models.ResourceGroups;

namespace Microsoft.Azure.Commands.Resources.Models
{
    public class ResourceIdentifier
    {
        public string ResourceType { get; set; }

        public string ResourceGroupName { get; set; }

        public string ResourceName { get; set; }

        [Obsolete("This property is obsolete. Please use Id instead.")]
        public string ParentResource { get; set; }
        public string Id { get; set; }

        public string Subscription { get; set; }

        public ResourceIdentifier() { }

        public ResourceIdentifier(string resourceId)
        {
            if (!string.IsNullOrEmpty(resourceId))
            {
                Id = resourceId;
                string[] tokens = StringExtensions.GetTokens(resourceId);
                if (tokens.Length < 8)
                {
                    throw new ArgumentException(ProjectResources.InvalidFormatOfResourceId, "idFromServer");
                }
                Subscription = tokens[1];
                ResourceGroupName =StringExtensions.GetValue(resourceId,3);
                ResourceName = StringExtensions.GetValue(resourceId, tokens.Length-1);
                ResourceType = StringExtensions.GetValue(resourceId, tokens.Length-2);
            }
        }
       
        public static string GetParentResource(string resourceType,string resourceName)
        {
            string[] tokensOfResourceType = resourceType.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string[] tokensOfResourceName = resourceName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> parentResourceBuilder = new List<string>();
            int sizeOfResourceTypeToken = tokensOfResourceType.Length;
            int sizeOfResourceNameToken = tokensOfResourceName.Length;
            if ((sizeOfResourceNameToken == sizeOfResourceTypeToken - 1))
            {
                if ((tokensOfResourceType.Length > 2) && (tokensOfResourceName.Length > 1))
                {
                    for (int index = 1; index < sizeOfResourceTypeToken - 1; index++)
                    {
                        parentResourceBuilder.Add(tokensOfResourceType[index]);
                        parentResourceBuilder.Add(tokensOfResourceName[index - 1]);
                    }
                }
            }
            else
            {
                throw new ArgumentException("Resource type or resource name is incorrect!");
            }

            return (parentResourceBuilder.Count != 0) ? string.Join("/", parentResourceBuilder) : null;
        }

        public override string ToString()
        {
            StringBuilder resourceId = new StringBuilder();
            if (!string.IsNullOrEmpty(Id))
            {
                return Id;
            }

            else
            {
                //ResourceType: Microsoft.Web/Sites/Slot/Extension
                //ResourceName: mysite/myslot/myextension
                string provider = StringExtensions.GetValue(ResourceType,0);
                string type = StringExtensions.GetValue(ResourceType, ResourceType.Length-1);
                string parentResource = GetParentResource(ResourceType, ResourceName);
                string parentAndType = string.IsNullOrEmpty(parentResource) ? type : parentResource + "/" + type;
                
                AppendIfNotNull(ref resourceId, "/subscriptions/{0}", Subscription);
                AppendIfNotNull(ref resourceId, "/resourceGroups/{0}", ResourceGroupName);
                AppendIfNotNull(ref resourceId, "/providers/{0}", provider);
                AppendIfNotNull(ref resourceId, "/{0}", parentAndType);
                AppendIfNotNull(ref resourceId, "/{0}", ResourceName);
                return resourceId.ToString();
            }
           
        }

        public AuthorizationResourceIdentity ToResourceIdentity()
        {
            AuthorizationResourceIdentity identity = null;

            if (!string.IsNullOrEmpty(ResourceType) && ResourceType.IndexOf('/') > 0)
            {
                int lengthOfType = StringExtensions.GetTokenSize(ResourceType);
                int lengthOfName = StringExtensions.GetTokenSize(ResourceName);

                identity = new AuthorizationResourceIdentity
                {
                    ResourceName = StringExtensions.GetValue(ResourceName, lengthOfName - 1),
                    ParentResourcePath = GetParentResource(ResourceType, ResourceName),
                    ResourceProviderNamespace = StringExtensions.GetValue(ResourceType, 0),
                    ResourceType = StringExtensions.GetValue(ResourceType, lengthOfType - 1)
                };
            }
            else
            {
                int length = StringExtensions.GetTokens(Id).Length;
                identity = new AuthorizationResourceIdentity
                {
                    ResourceName = StringExtensions.GetValue(Id, length - 1),
                    ParentResourcePath = GetParentResource(Id),
                    ResourceProviderNamespace = StringExtensions.GetValue(Id, 5),
                    ResourceType = StringExtensions.GetValue(Id, length - 2),
                 };
            }

            return identity;
        }

        public ResourcesResourceIdentity ToResourceIdentity(string apiVersion)
        {
            var identity = new ResourceIdentity();
            if (!string.IsNullOrEmpty(ResourceType) && ResourceType.IndexOf('/') > 0)
            {
                int lengthOfType = StringExtensions.GetTokenSize(ResourceType);
                int lengthOfName = StringExtensions.GetTokenSize(ResourceName);
                identity.ResourceName = StringExtensions.GetValue(ResourceName, lengthOfName - 1);
                identity.ParentResourcePath = GetParentResource(ResourceType, ResourceName);
                identity.ResourceProviderNamespace = StringExtensions.GetValue(ResourceType, 0);
                identity.ResourceType = StringExtensions.GetValue(ResourceType, lengthOfType - 1);
                identity.ResourceProviderApiVersion = apiVersion;
            }
            else
            {
                int length = StringExtensions.GetTokens(Id).Length;
                identity.ResourceName = StringExtensions.GetValue(Id, length - 1);
                identity.ParentResourcePath = GetParentResource(Id);
                identity.ResourceProviderNamespace = StringExtensions.GetValue(Id, 5);
                identity.ResourceType = StringExtensions.GetValue(Id, length - 2);
                identity.ResourceProviderApiVersion = apiVersion;
            }
            return identity;
        }

        private string GetParentResource(string resourceId)
        {
            string[] tokens = StringExtensions.GetTokens(resourceId);
            int length = tokens.Length;
            string res =null;

            if(length>8)
            {
                for (int i = 6; i <= length - 3; i++)
                    res += tokens[i] + "/";
            }
            return res==null?null:res.Substring(0,res.Length-1);
        }

        private void AppendIfNotNull(ref StringBuilder resourceId, string format, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                resourceId.AppendFormat(format, value);
            }
        }

        public static string GetResourceGroupName(string resourceId)
        {
            return StringExtensions.GetValue(resourceId, 3);
        }
    }
}
