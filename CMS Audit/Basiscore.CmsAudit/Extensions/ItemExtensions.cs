namespace Basiscore.CmsAudit.Extensions
{
    using Sitecore.Data.Items;

    public static class ItemExtensions
    {
        /// <summary>
        /// Returns the value of the phrase field
        /// Works for dictionary item only, i.e item created with DictionaryEntry template
        /// </summary>        
        public static string GetDictionaryPhrase(this Item item)
        {
            string phrase = string.Empty;

            if (item != null && item.TemplateID == Templates.DictionaryEntry.ID)
            {
                phrase = item.Fields[Templates.DictionaryEntry.Fields.Phrase].Value;
            }

            return phrase;
        }
    }
}