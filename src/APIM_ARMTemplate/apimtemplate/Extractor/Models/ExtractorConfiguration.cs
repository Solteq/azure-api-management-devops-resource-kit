using System;
using System.ComponentModel;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Extract
{
    public class ExtractorConfig
    {
        [Description("Source API Management name")]
        public string sourceApimName { get; set; }
        [Description("Destination API Management name")]
        public string destinationApimName { get; set; }
        [Description("Resource Group name")]
        public string resourceGroup { get; set; }
        [Description("ARM Template files folder")]
        public string fileFolder { get; set; }
        [Description("API name")]
        public string apiName { get; set; }
        [Description("Comma-separated list of API names")]
        public string mutipleAPIs { get; set; }
        [Description("Creates a master template with links")]
        public string linkedTemplatesBaseUrl { get; set; }
        public string linkedTemplatesSasToken { get; set; }
        [Description("Query string appended to linked templates uris that enables retrieval from private storage")]
        public string linkedTemplatesUrlQueryString { get; set; }
        [Description("Writes policies to local XML files that require deployment to remote folder")]
        public string policyXMLBaseUrl { get; set; }
        [Description("String appended to end of the linked templates uris that enables adding a SAS token or other query parameters")]
        public string policyXMLSasToken { get; set; }
        [Description("Split APIs into multiple templates")]
        public string splitAPIs { get; set; }
        [Description("Name of the apiVersionSet you want to extract")]
        public string apiVersionSetName { get; set; }
        [Description("Includes all revisions for a single api - use with caution")]
        public string includeAllRevisions { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(sourceApimName)) throw new ArgumentException("Missing parameter <sourceApimName>.");
            if (string.IsNullOrEmpty(destinationApimName)) throw new ArgumentException("Missing parameter <destinationApimName>.");
            if (string.IsNullOrEmpty(resourceGroup)) throw new ArgumentException("Missing parameter <resourceGroup>.");
            if (string.IsNullOrEmpty(fileFolder)) throw new ArgumentException("Missing parameter <filefolder>.");

            bool shouldSplitAPIs = splitAPIs != null && splitAPIs.Equals("true");
            bool hasVersionSetName = apiVersionSetName != null;
            bool hasSingleApi = apiName != null;
            bool includeRevisions = includeAllRevisions != null && includeAllRevisions.Equals("true");
            bool hasMultipleAPIs = mutipleAPIs != null;

            if (shouldSplitAPIs && hasSingleApi)
            {
                throw new NotSupportedException("Can't use splitAPIs and apiName at same time");
            }

            if (shouldSplitAPIs && hasVersionSetName)
            {
                throw new NotSupportedException("Can't use splitAPIs and apiVersionSetName at same time");
            }

            if ((hasVersionSetName || hasSingleApi) && hasMultipleAPIs)
            {
                throw new NotSupportedException("Can't use mutipleAPIs with apiName or apiVersionSetName at the same time");
            }

            if (hasSingleApi && hasVersionSetName)
            {
                throw new NotSupportedException("Can't use apiName and apiVersionSetName at same time");
            }

            if (!hasSingleApi && includeRevisions)
            {
                throw new NotSupportedException("\"includeAllRevisions\" can be used when you specify the API you want to extract with \"apiName\"");
            }
        }
    }

    public class Extractor
    {
        public string sourceApimName { get; private set; }
        public string destinationApimName { get; private set; }
        public string resourceGroup { get; private set; }
        public string fileFolder { get; private set; }
        public string linkedTemplatesBaseUrl { get; private set; }
        public string linkedTemplatesSasToken { get; private set; }
        public string linkedTemplatesUrlQueryString { get; private set; }
        public string policyXMLBaseUrl { get; private set; }
        public string policyXMLSasToken { get; private set; }
        public string apiVersionSetName { get; private set; }
        public bool includeAllRevisions { get; private set; }

        public Extractor(ExtractorConfig exc, string dirName)
        {
            this.sourceApimName = exc.sourceApimName;
            this.destinationApimName = exc.destinationApimName;
            this.resourceGroup = exc.resourceGroup;
            this.fileFolder = dirName;
            this.linkedTemplatesBaseUrl = exc.linkedTemplatesBaseUrl;
            this.linkedTemplatesSasToken = exc.linkedTemplatesSasToken;
            this.linkedTemplatesUrlQueryString = exc.linkedTemplatesUrlQueryString;
            this.policyXMLBaseUrl = exc.policyXMLBaseUrl;
            this.policyXMLSasToken = exc.policyXMLSasToken;
            this.apiVersionSetName = exc.apiVersionSetName;
            this.includeAllRevisions = exc.includeAllRevisions != null && exc.includeAllRevisions.Equals("true");
        }

        public Extractor(ExtractorConfig exc): this(exc, exc.fileFolder)
        {
        }
    }
}