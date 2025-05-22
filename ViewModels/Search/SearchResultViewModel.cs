namespace Pz_Proj_11_12.ViewModels.Search
{
	public class SearchResultViewModel
	{
        public string SearchQuery { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public Pagination<SearchViewModel> Results { get; set; }
    }
}
