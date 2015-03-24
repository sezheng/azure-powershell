﻿// ----------------------------------------------------------------------------------
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
using System.Text;
using AuthorizationResourceIdentity = Microsoft.Azure.ResourceIdentity;
using ProjectResources = Microsoft.Azure.Commands.Resources.Properties.Resources;
using ResourcesResourceIdentity = Microsoft.Azure.ResourceIdentity;

namespace Microsoft.Azure.Commands.Resources.Models
{
    public class ResourceIdentifier
    {
        public string ResourceType { get; set; }

        public string ResourceGroupName { get; set; }

        public string ResourceName { get; set; }
        
        public string Id { get; set; }

        public string Subscription { get; set; }

        public ResourceIdentifier() { }

        public ResourceIdentifier(string idFromServer)
        {
            if (!string.IsNullOrEmpty(idFromServer))
            {
                Id = idFromServer;
                string[] tokens = idFromServer.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 8)
                {
                    throw new ArgumentException(ProjectResources.InvalidFormatOfResourceId, "idFromServer");
                }
                Subscription = tokens[1];
                ResourceGroupName = tokens[3];
                ResourceName = tokens[tokens.Length - 1];

                List<string> resourceTypeBuilder = new List<string>();
                resourceTypeBuilder.Add(tokens[5]);
                
                List<string> parentResourceBuilder = new List<string>();
                for (int i = 6; i <= tokens.Length - 3; i++)
                {
                    parentResourceBuilder.Add(tokens[i]);
                    // Add every other token to type
                    if (i%2 == 0)
                    {
                        resourceTypeBuilder.Add(tokens[i]);
                    }
                }
                resourceTypeBuilder.Add(tokens[tokens.Length - 2]);

                if (parentResourceBuilder.Count > 0)
                {
                   //ParentResource = string.Join("/", parentResourceBuilder);
                    ResourceName = string.Join("/", parentResourceBuilder);
                }
                if (resourceTypeBuilder.Count > 0)
                {
                    ResourceType = string.Join("/", resourceTypeBuilder);
                }
            }
        }

        public static string GetParentResource(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            else
            {
                string parentResource = string.Empty;
            
                string[] tokens = id.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 8)
                {
                    throw new ArgumentException(ProjectResources.InvalidFormatOfResourceId, "idFromServer");
                }
                List<string> parentResourceBuilder = new List<string>();
                for (int i = 6; i <= tokens.Length - 3; i++)
                {
                    parentResourceBuilder.Add(tokens[i]);
                }

                if (parentResourceBuilder.Count > 0)
                {
                    parentResource = string.Join("/", parentResourceBuilder);
                }
                return (parentResource==String.Empty)?null:parentResource;
            }
        }

        public static string GetResourceGroupName(string id)
        {
            if (id == null)
            {
                return null;
            }

            string[] tokens = id.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 8)
            {
                throw new ArgumentException(ProjectResources.InvalidFormatOfResourceId, "idFromServer");
            }
            return tokens[3];
        }

        public static string GetProviderFromResourceType(string resourceType)
        {
            if (resourceType == null)
            {
                return null;
            }

            int indexOfSlash = resourceType.IndexOf('/');
            if (indexOfSlash < 0)
            {
                return string.Empty;
            }
            else
            {
                return resourceType.Substring(0, indexOfSlash);
            }
        }

        public static string GetTypeFromResourceType(string resourceType)
        {
            if (resourceType == null)
            {
                return null;
            }

            int lastIndexOfSlash = resourceType.LastIndexOf('/');
            if (lastIndexOfSlash < 0)
            {
                return string.Empty;
            }
            else
            {
                return resourceType.Substring(lastIndexOfSlash + 1);
            }
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
                string provider = GetProviderFromResourceType(ResourceType);
                string type = GetTypeFromResourceType(ResourceType);
                string parentResource = GetParentResource(Id);
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
                identity = new AuthorizationResourceIdentity
                {
                    ResourceName = ResourceName,
                    ParentResourcePath = GetParentResource(Id),
                    ResourceProviderNamespace = ResourceIdentifier.GetProviderFromResourceType(ResourceType),
                    ResourceType = ResourceIdentifier.GetTypeFromResourceType(ResourceType)
                };
            }

            return identity;
        }

        public ResourcesResourceIdentity ToResourceIdentity(string apiVersion)
        {
            if (string.IsNullOrEmpty(ResourceType))
            {
                throw new ArgumentNullException("ResourceType");
            }
            if (ResourceType.IndexOf('/') < 0)
            {
                throw new ArgumentException(ProjectResources.ResourceTypeFormat, "ResourceType");
            }

            ResourcesResourceIdentity identity = new ResourcesResourceIdentity
            {
                ResourceName = ResourceName,
                ParentResourcePath = GetParentResource(Id),
                ResourceProviderNamespace = ResourceIdentifier.GetProviderFromResourceType(ResourceType),
                ResourceType = ResourceIdentifier.GetTypeFromResourceType(ResourceType),
                ResourceProviderApiVersion = apiVersion
            };

            return identity;
        }
        
        private void AppendIfNotNull(ref StringBuilder resourceId, string format, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                resourceId.AppendFormat(format, value);
            }
        }
    }
}
