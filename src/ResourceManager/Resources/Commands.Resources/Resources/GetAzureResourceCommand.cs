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
using Microsoft.Azure.Commands.Resources.Models;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using ProjectResources = Microsoft.Azure.Commands.Resources.Properties.Resources;

namespace Microsoft.Azure.Commands.Resources
{
    /// <summary>
    /// Get an existing resource.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureResource", DefaultParameterSetName = BaseParameterSetName), OutputType(typeof(PSResource))]
    public class GetAzureResourceCommand : ResourcesBaseCmdlet
    {
        internal const string BaseParameterSetName = "List resources";
        internal const string ParameterSetNameWithTypeAndName = "Get a single resource by type and name";
        internal const string ParameterSetNameWithId = "Get a single resource by Id";

        [Alias("ResourceName")]
        [Parameter(ParameterSetName = ParameterSetNameWithTypeAndName, Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = ParameterSetNameWithTypeAndName, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group name.")]
        [Parameter(ParameterSetName = BaseParameterSetName, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group name.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(ParameterSetName = ParameterSetNameWithTypeAndName, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource type. In the format ResourceProvider/type.")]
        [Parameter(ParameterSetName = BaseParameterSetName, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource type. In the format ResourceProvider/type.")]
        [ValidateNotNullOrEmpty]
        public string ResourceType { get; set; }

        [Parameter(ParameterSetName = BaseParameterSetName, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource tags.")]
        public Hashtable Tag { get; set; }

        [Alias("ResourceId")]
        [Parameter(ParameterSetName = ParameterSetNameWithId, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full qualified Id of the resource, in the format of subscriptionId/{subscriptionId}/resourceGroups/{resourceGroup}/{resourceType}/{resourceName}")]
        public string Id { get; set; }

        [Obsolete("This parameter is obsolete. Please use Id instead.")]
        [Parameter(ParameterSetName = ParameterSetNameWithTypeAndName, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the parent resource if needed. In the format of greatgrandpa/grandpa/dad.")]
        public string ParentResource { get; set; }

        [Parameter(ParameterSetName = ParameterSetNameWithTypeAndName, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Version of the resource provider API.")]
        [Parameter(ParameterSetName = ParameterSetNameWithId, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Version of the resource provider API.")]
        [ValidateNotNullOrEmpty]
        public string ApiVersion { get; set; }

        public override void ExecuteCmdlet()
        {
            if(!string.IsNullOrEmpty(ParentResource))
            {
                WriteWarning(ProjectResources.ParentResourceIsDeprecated);
            }

            BasePSResourceParameters parameters = new BasePSResourceParameters()
            {
                Name = Name,
                ResourceGroupName = ResourceGroupName,
                ResourceType = ResourceType,
                Id = Id,
                ParentResource = ParentResource,
                ApiVersion = ApiVersion,
                Tag = new[] { Tag }
            };

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
                        "ParentResource", r.ParentResource,
                        "Location", r.Location,
                        "Permissions", r.PermissionsTable,
                        "ResourceId", r.ResourceId)));

                    WriteObject(output, true);
                }
            }
        }
    }
}
