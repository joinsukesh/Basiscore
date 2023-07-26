namespace Basiscore.CmsAudit.Models
{
    public class Pagination
    {
        public bool ShowPagination { get; set; }
        public int PageSize { get; set; }
        public int SelectedPage { get; set; }
        public int TotalPages { get; set; }

        public static int GetTotalPages(int totalRecords, int maxRecordsPerPage)
        {
            int totalPages = 1;

            if (totalRecords > 0 && maxRecordsPerPage > 0)
            {
                totalPages = (totalRecords + maxRecordsPerPage - 1) / maxRecordsPerPage;
            }

            return totalPages;
        }

        /// <summary>
        /// this will return the start index to later use to get items from a list, from that index.
        /// e.g. list.GetRange(startIndex, numberOfItemsToGet)
        /// </summary>
        /// <param name="selectedPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static int GetStartIndex(int selectedPage, int pageSize, int totalItems)
        {
            int startIndex = selectedPage > 1 ? ((selectedPage * pageSize) - pageSize) : 0;
            return startIndex >= totalItems ? (totalItems - 1) : startIndex;
        }

        /// <summary>
        /// this will return the valid number of items to get from list.
        /// Because the default pageSize number of items may be unavailable
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalItems"></param>
        /// <returns></returns>
        public static int GetNumberOfItemsToFetchFromList(int startIndex, int pageSize, int totalItems)
        {
            int validPageSize = (startIndex + pageSize) > totalItems ? totalItems - startIndex : pageSize;
            return validPageSize < 0 ? 0 : validPageSize;
        }

        //public string ListContainerId { get; set; }
        //public string DatasourceId { get; set; }
    }
}