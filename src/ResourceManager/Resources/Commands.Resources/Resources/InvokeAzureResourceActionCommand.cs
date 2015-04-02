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

using Microsoft.Azure.Commands.Resources.Models;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.WindowsAzure.Commands.Websites;
using Microsoft.WindowsAzure.Commands.Utilities.Websites;
using System;

namespace Microsoft.Azure.Commands.Resources
{
    /// <summary>
    /// Get an existing resource.
    /// </summary>
    //[Cmdlet(VerbsLifecycle.Invoke, "AzureResource"), OutputType(typeof(bool))]
    [Cmdlet(VerbsLifecycle.Invoke, "AzureResourceAction"),OutputType(typeof(PSResource))]
    public class InvokeAzureResourceActionCommand : ResourcesBaseCmdlet
    {       
        [Alias("ResourceName")]
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group name.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource type name.")]
        [ValidateNotNullOrEmpty]
        public string ResourceType { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The action name.")]
        [ValidateNotNullOrEmpty]
        public string ActionName { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The api version.")]
        [ValidateNotNullOrEmpty]
        public string ApiVersion { get; set; }

        public override void ExecuteCmdlet()
        {
            BasePSResourceParameters parameters = new BasePSResourceParameters()
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                ResourceType = ResourceType,
                ApiVersion = ApiVersion
            };            

            switch(ActionName.ToLowerInvariant())
            {
                case "list":
                    List<PSResource> resourceList = ResourcesClient.FilterPSResources(parameters);
                    if (resourceList != null)
                    {
                        if (resourceList.Count == 1 && Name != null)
                        {
                            WriteObject(resourceList[0]);
                        }
                        else
                        {
                            List<PSObject> output = new List<PSObject>();
                            resourceList.ForEach(r => output.Add(base.ConstructPSObject(
                                null,
                                "Name", r.Name,
                                "ResourceGroupName", r.ResourceGroupName,
                                "ResourceType", r.ResourceType,
                                "Location", r.Location,
                                "Permissions", r.PermissionsTable,
                                "ResourceId", r.ResourceId)));

                            WriteObject(output, true);
                        }
                    }
                   break;
                case "stop":
                   WebsitesClient = WebsitesClient ?? new WebsitesClient(Profile, Profile.Context.Subscription, WriteDebug);
                   WebsitesClient.StopWebsite(Name);                                  
                   break;
                default:
                   throw new ApplicationException(string.Format("Unknown action encountered: '{0}'", ActionName));

            }            
        }

        
    }
}
