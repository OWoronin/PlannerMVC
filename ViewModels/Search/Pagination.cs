using Microsoft.EntityFrameworkCore;

//from https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/?view=aspnetcore-8.0


namespace Pz_Proj_11_12.ViewModels.Search
{
	public class Pagination<T> : List<T>
	{
		public int PageId { get; set; }
		public int PagesTotal { get; set; }

		public Pagination(List<T> elForPage, int pageId, int pageSize, int counter)
		{
			PageId = pageId;
			PagesTotal = (int)Math.Ceiling(counter/(double)pageSize);
			AddRange(elForPage); 
		}

		public bool previousPage => PageId > 1; 
		public bool nextPage => PageId < PagesTotal;

		public static async Task<Pagination<T>> Create(IQueryable<T> source, int pageId, int pageSize)
		{
			var counter = await source.CountAsync();
			var elements = await source.Skip((pageId - 1) * pageSize).Take(pageSize).ToListAsync();
			return new Pagination<T>(elements, pageId, pageSize, counter);
		}




	}
}
