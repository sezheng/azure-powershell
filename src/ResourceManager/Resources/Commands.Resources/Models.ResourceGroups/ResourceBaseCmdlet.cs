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
using Microsoft.Azure.Commands.Resources.Models;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Resources
{
    public abstract class ResourceBaseCmdlet : ResourcesBaseCmdlet
    {
        internal const string ParameterSetNameWithTypeAndName = "Single resource identified by type and name";
        internal const string ParameterSetNameWithId = "Single resource identified by Id";

        [Alias("ResourceName")]
        [Parameter(ParameterSetName = ParameterSetNameWithTypeAndName,Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource name.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ParameterSetName = ParameterSetNameWithTypeAndName,Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group name.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(ParameterSetName = ParameterSetNameWithTypeAndName,Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource type. In the format ResourceProvider/type.")]
        [ValidateNotNullOrEmpty]
        public string ResourceType { get; set; }

        [Obsolete("This parameter is obsolete.")]
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The name of the parent resource if needed. In the format of greatgranda/grandpa/dad.")]
        public string ParentResource { get; set; }

        [Parameter(ParameterSetName = ParameterSetNameWithId, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource Id.")]
        [ValidateNotNullOrEmpty]
        public string Id { get; set; }
    }
}
