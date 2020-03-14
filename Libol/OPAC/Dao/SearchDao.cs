using OPAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Text;
using System.Text.RegularExpressions;
using OPAC.Models.SupportClass;

namespace OPAC.Dao
{
    public class SearchDao
    {
        /// <summary>
        /// Search book with key word
        /// </summary>
        /// <param name="searchKeyword"></param>
        /// <param name="option"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_Result> GetSearchingBook(string searchKeyword, string option, int page, int pageSize)
        {
            using (var dbContext = new OpacEntities())
            {
                searchKeyword = Regex.Replace(searchKeyword, @"\s+", " ").Trim();
                var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_Result>("FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK {0}, {1}",
                     new object[] { searchKeyword, option }).ToList();

                return list.ToPagedList(page, pageSize);
            }
        }

        /// <summary>
        /// Get full search book list
        /// </summary>
        /// <param name="searchKeyword"></param>
        /// <param name="option"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_Result> GetFullSearchingBook(string searchKeyword, string option)
        {
            using (var dbContext = new OpacEntities())
            {
                searchKeyword = Regex.Replace(searchKeyword, @"\s+", " ").Trim();
                var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_Result>(
                    "FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK {0}, {1}",
                    new object[] { searchKeyword, option }).ToList();

                return list;
            }
        }

        /// <summary>
        /// Search book by key word
        /// </summary>
        /// <param name="searchKeyword"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchBy"></param>
        /// <returns></returns>
        public IEnumerable<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result> GetSearchingBookByKeyWord(string searchKeyword, int page, int pageSize, int searchBy)
        {
            /*
             * Parameter searchBy definition:
             * 1: Search by keyword
             * 2: Search by DDC
             */
            using (var dbContext = new OpacEntities())
            {
                var listResult = new List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>();
                if (searchBy == 1)
                {
                    var getItemID = dbContext.FPT_SP_GET_ITEMID_BY_KEYWORD(searchKeyword).ToList();

                    foreach (var item in getItemID)
                    {
                        var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>("FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD {0}",
                            new object[] { item }).ToList();

                        listResult.AddRange(list);
                    }

                    var uniqueIdList = listResult.Select(t => t.ID).Distinct().ToList();
                    var indexIdList = new List<int>();
                    var duplicatesIdList = listResult.Select((t, i) => new { Index = i, ItemID = t.ID }).GroupBy(g => g.ItemID)
                        .Where(g => g.Count() > 1);

                    foreach (var idList in duplicatesIdList)
                    {
                        int count = 0;
                        foreach (var item in idList)
                        {
                            count++;
                            if (count > 1) //Only find value have index > 0
                            {
                                foreach (var item1 in uniqueIdList) //Get index of duplicated value
                                {
                                    if (item.ItemID == item1)
                                    {
                                        indexIdList.Add(item.Index);
                                    }
                                }
                            }
                        }
                    }

                    //Get duplicated values
                    var duplicatedList = new List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>();
                    foreach (var index in indexIdList)
                    {
                        duplicatedList.Add(listResult[index]);
                    }

                    //Remove all duplicated values
                    foreach (var item in duplicatedList)
                    {
                        listResult.Remove(item);
                    }
                }
                else if (searchBy == 2)
                {
                    var getItemID = dbContext.FPT_SP_OPAC_GET_ITEMID_BY_DDC(searchKeyword).ToList();

                    foreach (var item in getItemID)
                    {
                        var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>("FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD {0}",
                            new object[] { item }).ToList();

                        listResult.AddRange(list);
                    }

                    var uniqueIdList = listResult.Select(t => t.ID).Distinct().ToList();
                    var indexIdList = new List<int>();
                    var duplicatesIdList = listResult.Select((t, i) => new { Index = i, ItemID = t.ID }).GroupBy(g => g.ItemID)
                        .Where(g => g.Count() > 1);

                    foreach (var idList in duplicatesIdList)
                    {
                        int count = 0;
                        foreach (var item in idList)
                        {
                            count++;
                            if (count > 1) //Only find value have index > 0
                            {
                                foreach (var item1 in uniqueIdList) //Get index of duplicated value
                                {
                                    if (item.ItemID == item1)
                                    {
                                        indexIdList.Add(item.Index);
                                    }
                                }
                            }
                        }
                    }

                    //Get duplicated values
                    var duplicatedList = new List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>();
                    foreach (var index in indexIdList)
                    {
                        duplicatedList.Add(listResult[index]);
                    }

                    //Remove all duplicated values
                    foreach (var item in duplicatedList)
                    {
                        listResult.Remove(item);
                    }
                }

                return listResult.ToPagedList(page, pageSize);
            }
        }

        /// <summary>
        /// Get full search book list
        /// </summary>
        /// <param name="searchKeyword"></param>
        /// <param name="searchBy"></param>
        /// <returns></returns>
        public IEnumerable<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result> GetFullSearchingBookByKeyWord(string searchKeyword, int searchBy)
        {
            /*
             * Parameter searchBy definition:
             * 1: Search by keyword
             * 2: Search by DDC
             */
            using (var dbContext = new OpacEntities())
            {
                var listResult = new List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>();
                if (searchBy == 1)
                {
                    var getItemID = dbContext.FPT_SP_GET_ITEMID_BY_KEYWORD(searchKeyword).ToList();

                    foreach (var item in getItemID)
                    {
                        var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>("FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD {0}",
                            new object[] { item }).ToList();

                        listResult.AddRange(list);
                    }

                    var uniqueIdList = listResult.Select(t => t.ID).Distinct().ToList();
                    var indexIdList = new List<int>();
                    var duplicatesIdList = listResult.Select((t, i) => new { Index = i, ItemID = t.ID }).GroupBy(g => g.ItemID)
                        .Where(g => g.Count() > 1);

                    foreach (var idList in duplicatesIdList)
                    {
                        int count = 0;
                        foreach (var item in idList)
                        {
                            count++;
                            if (count > 1) //Only find value have index > 0
                            {
                                foreach (var item1 in uniqueIdList) //Get index of duplicated value
                                {
                                    if (item.ItemID == item1)
                                    {
                                        indexIdList.Add(item.Index);
                                    }
                                }
                            }
                        }
                    }

                    //Get duplicated values
                    var duplicatedList = new List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>();
                    foreach (var index in indexIdList)
                    {
                        duplicatedList.Add(listResult[index]);
                    }

                    //Remove all duplicated values
                    foreach (var item in duplicatedList)
                    {
                        listResult.Remove(item);
                    }
                }
                else if (searchBy == 2)
                {
                    var getItemID = dbContext.FPT_SP_OPAC_GET_ITEMID_BY_DDC(searchKeyword).ToList();

                    foreach (var item in getItemID)
                    {
                        var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>("FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD {0}",
                            new object[] { item }).ToList();

                        listResult.AddRange(list);
                    }

                    var uniqueIdList = listResult.Select(t => t.ID).Distinct().ToList();
                    var indexIdList = new List<int>();
                    var duplicatesIdList = listResult.Select((t, i) => new { Index = i, ItemID = t.ID }).GroupBy(g => g.ItemID)
                        .Where(g => g.Count() > 1);

                    foreach (var idList in duplicatesIdList)
                    {
                        int count = 0;
                        foreach (var item in idList)
                        {
                            count++;
                            if (count > 1) //Only find value have index > 0
                            {
                                foreach (var item1 in uniqueIdList) //Get index of duplicated value
                                {
                                    if (item.ItemID == item1)
                                    {
                                        indexIdList.Add(item.Index);
                                    }
                                }
                            }
                        }
                    }

                    //Get duplicated values
                    var duplicatedList = new List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>();
                    foreach (var index in indexIdList)
                    {
                        duplicatedList.Add(listResult[index]);
                    }

                    //Remove all duplicated values
                    foreach (var item in duplicatedList)
                    {
                        listResult.Remove(item);
                    }
                }

                return listResult;
            }
        }


        public List<FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID_Result> GetBookList(string input, string searchType,
            List<FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID_Result> listBookWithCondition)
        {
            var list = new List<FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID_Result>();

            switch (searchType)
            {
                case "AllFields":
                    foreach (var item in listBookWithCondition)
                    {
                        foreach (var pi in item.GetType().GetProperties())
                        {
                            if (pi.PropertyType == typeof(string))
                            {
                                if (pi.GetValue(item) != null)
                                {
                                    string value = pi.GetValue(item).ToString();
                                    if (value.ToLower().Contains(input.ToLower()))
                                    {
                                        list.Add(item);
                                    }
                                }
                            }
                        }
                    }

                    break;
                case "Title":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Title) &&
                                                                      item.Title.ToLower().Contains(input.ToLower())));

                    break;
                case "1":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Author) &&
                                                                      item.Author.ToLower().Contains(input.ToLower())));

                    break;
                case "2":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Publisher) &&
                                                                      item.Publisher.ToLower().Contains(input.ToLower())));

                    break;
                case "3":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.KeyWord) &&
                                                                      item.KeyWord.ToLower().Contains(input.ToLower())));

                    break;
                case "4":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.BBK) &&
                                                                      item.BBK.ToLower().Contains(input.ToLower())));

                    break;
                case "5":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.DDC) &&
                                                                      item.DDC.ToLower().Contains(input.ToLower())));

                    break;
                case "6":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.LOC) &&
                                                                      item.LOC.ToLower().Contains(input.ToLower())));

                    break;
                case "7":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.UDC) &&
                                                                      item.UDC.ToLower().Contains(input.ToLower())));

                    break;
                case "9":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.SH) &&
                                                                      item.SH.ToLower().Contains(input.ToLower())));

                    break;
                case "10":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Language) &&
                                                                      item.Language.ToLower().Contains(input.ToLower())));

                    break;
                case "11":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Country) &&
                                                                      item.Country.ToLower().Contains(input.ToLower())));

                    break;
                case "12":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Series) &&
                                                                      item.Series.ToLower().Contains(input.ToLower())));

                    break;
                case "19":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Thesis_Subject) &&
                                                                      item.Thesis_Subject.ToLower().Contains(input.ToLower())));

                    break;
                case "30":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.NLM) &&
                                                                      item.NLM.ToLower().Contains(input.ToLower())));

                    break;
                case "31":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.OAI_SET) &&
                                                                      item.OAI_SET.ToLower().Contains(input.ToLower())));

                    break;
                case "40":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Dictionary40) &&
                                                                      item.Dictionary40.ToLower().Contains(input.ToLower())));

                    break;
                case "41":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Dictionary41) &&
                                                                      item.Dictionary41.ToLower().Contains(input.ToLower())));

                    break;
                case "42":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Dictionary42) &&
                                                                      item.Dictionary42.ToLower().Contains(input.ToLower())));

                    break;
                case "43":
                    list.AddRange(listBookWithCondition.Where(item => !string.IsNullOrEmpty(item.Dictionary43) &&
                                                                      item.Dictionary43.ToLower().Contains(input.ToLower())));

                    break;
                default:
                    break;
            }

            return list.Distinct().ToList();
        }

        /// <summary>
        /// Get result of advanced search function
        /// </summary>
        /// <param name="input"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result> GetAdvancedSearchBook(AdvancedSearch input, int page, int pageSize)
        {
            var list = new List<FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result>();
            var deleteList = new List<FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result>();
            bool flagForCondition1 = false;
            bool flagForCondition2 = false;
            bool flagForCondition3 = false;

            using (var dbContext = new OpacEntities())
            {
                if (!string.IsNullOrEmpty(input.TxtSearch1.Trim()))
                {
                    var listBook = dbContext.FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID(input.LibraryId,
                        input.LocationId, input.DocumentType).ToList();
                    var tempList = GetBookList(input.TxtSearch1, input.SearchType1, listBook);

                    if (!string.IsNullOrEmpty(input.TxtSearch2.Trim()))
                    {
                        if (input.Condition1.Equals("and"))
                        {
                            tempList = GetBookList(input.TxtSearch2, input.SearchType2, tempList);
                            flagForCondition1 = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(input.TxtSearch3.Trim()))
                    {
                        if (input.Condition2.Equals("and"))
                        {
                            tempList = GetBookList(input.TxtSearch3, input.SearchType3, tempList);
                            flagForCondition2 = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                    {
                        if (input.Condition3.Equals("and"))
                        {
                            tempList = GetBookList(input.TxtSearch4, input.SearchType4, tempList);
                            flagForCondition3 = true;
                        }
                    }

                    tempList = tempList.Distinct().ToList();

                    list.AddRange((from t in tempList
                        select new FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result()
                        {
                            ID = t.ID,
                            Title = t.Title,
                            Author = t.Author,
                            Publisher = t.Publisher,
                            Version = t.Version,
                            Year = t.Year
                        }).ToList());
                }

                if (!string.IsNullOrEmpty(input.TxtSearch2.Trim()))
                {
                    var tempList = dbContext.FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK(input.LibraryId, input.LocationId,
                        input.SearchType2, input.DocumentType, input.TxtSearch2).ToList();

                    if (!flagForCondition1)
                    {
                        if (input.Condition1.Equals("and"))
                        {
                            var listBook = dbContext.FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID(input.LibraryId,
                                input.LocationId, input.DocumentType).ToList();
                            var resultList = GetBookList(input.TxtSearch2, input.SearchType2, listBook);

                            if (!string.IsNullOrEmpty(input.TxtSearch3.Trim()))
                            {
                                if (input.Condition2.Equals("and"))
                                {
                                    resultList = GetBookList(input.TxtSearch3, input.SearchType3, resultList);
                                    flagForCondition2 = true;
                                }
                            }

                            if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                            {
                                if (input.Condition3.Equals("and"))
                                {
                                    resultList = GetBookList(input.TxtSearch4, input.SearchType4, resultList);
                                    flagForCondition3 = true;
                                }
                            }

                            resultList = resultList.Distinct().ToList();

                            list.AddRange((from t in resultList
                                select new FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result()
                                {
                                    ID = t.ID,
                                    Title = t.Title,
                                    Author = t.Author,
                                    Publisher = t.Publisher,
                                    Version = t.Version,
                                    Year = t.Year
                                }).ToList());
                        }
                    }

                    if (input.Condition1.Equals("or"))
                    {
                        list.AddRange(tempList);
                    }
                    else if (input.Condition1.Equals("not"))
                    {
                        deleteList.AddRange(tempList);
                    }
                }

                if (!string.IsNullOrEmpty(input.TxtSearch3.Trim()))
                {
                    var tempList = dbContext.FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK(input.LibraryId, input.LocationId,
                        input.SearchType3, input.DocumentType, input.TxtSearch3).ToList();

                    if (!flagForCondition2)
                    {
                        if (input.Condition2.Equals("and"))
                        {
                            var listBook = dbContext.FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID(input.LibraryId,
                                input.LocationId, input.DocumentType).ToList();
                            var resultList = GetBookList(input.TxtSearch3, input.SearchType3, listBook);

                            if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                            {
                                if (input.Condition3.Equals("and"))
                                {
                                    resultList = GetBookList(input.TxtSearch4, input.SearchType4, resultList);
                                    flagForCondition3 = true;
                                }
                            }

                            resultList = resultList.Distinct().ToList();

                            list.AddRange((from t in resultList
                                select new FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result()
                                {
                                    ID = t.ID,
                                    Title = t.Title,
                                    Author = t.Author,
                                    Publisher = t.Publisher,
                                    Version = t.Version,
                                    Year = t.Year
                                }).ToList());
                        }
                    }

                    if (input.Condition2.Equals("or"))
                    {
                        list.AddRange(tempList);
                    }
                    else if (input.Condition2.Equals("not"))
                    {
                        deleteList.AddRange(tempList);
                    }
                }

                if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                {
                    var tempList = dbContext.FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK(input.LibraryId, input.LocationId,
                        input.SearchType4, input.DocumentType, input.TxtSearch4).ToList();

                    if (!flagForCondition3)
                    {
                        if (input.Condition3.Equals("and"))
                        {
                            list.AddRange(tempList);
                        }
                    }

                    if (input.Condition3.Equals("or"))
                    {
                        list.AddRange(tempList);
                    }
                    else if (input.Condition3.Equals("not"))
                    {
                        deleteList.AddRange(tempList);
                    }
                }

                list = list.Distinct().ToList();
                
                foreach (var item in list.Distinct().ToList())
                {
                    if (deleteList.Any(deleteItem => deleteItem.ID == item.ID))
                    {
                        list.Remove(item);
                    }
                }

                /*
                 * OrderBy = 1 : Sort by title
                 * OrderBy = 2 : Sort by author
                 * OrderBy = 3 : Sort by published year
                 * OrderBy = 4 : Sort by publisher
                 */
                if (input.OrderBy.Equals("1"))
                {
                    list = list.OrderBy(t => t.Title).ToList();
                }
                else if (input.OrderBy.Equals("2"))
                {
                    list = list.OrderBy(t => t.Author).ToList();
                }
                else if (input.OrderBy.Equals("3"))
                {
                    list = list.OrderBy(t => t.Year).ToList();
                }
                else
                {
                    list = list.OrderBy(t => t.Publisher).ToList();
                }
            }

            return list.ToPagedList(page, pageSize);
        }

        /// <summary>
        /// Get full result of advanced search function
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IEnumerable<FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result> GetFullAdvancedSearchBook(AdvancedSearch input)
        {
            var list = new List<FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result>();
            var deleteList = new List<FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result>();
            bool flagForCondition1 = false;
            bool flagForCondition2 = false;
            bool flagForCondition3 = false;

            using (var dbContext = new OpacEntities())
            {
                if (!string.IsNullOrEmpty(input.TxtSearch1.Trim()))
                {
                    var listBook = dbContext.FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID(input.LibraryId,
                        input.LocationId, input.DocumentType).ToList();
                    var tempList = GetBookList(input.TxtSearch1, input.SearchType1, listBook);

                    if (!string.IsNullOrEmpty(input.TxtSearch2.Trim()))
                    {
                        if (input.Condition1.Equals("and"))
                        {
                            tempList = GetBookList(input.TxtSearch2, input.SearchType2, tempList);
                            flagForCondition1 = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(input.TxtSearch3.Trim()))
                    {
                        if (input.Condition2.Equals("and"))
                        {
                            tempList = GetBookList(input.TxtSearch3, input.SearchType3, tempList);
                            flagForCondition2 = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                    {
                        if (input.Condition3.Equals("and"))
                        {
                            tempList = GetBookList(input.TxtSearch4, input.SearchType4, tempList);
                            flagForCondition3 = true;
                        }
                    }

                    tempList = tempList.Distinct().ToList();

                    list.AddRange((from t in tempList
                                   select new FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result()
                                   {
                                       ID = t.ID,
                                       Title = t.Title,
                                       Author = t.Author,
                                       Publisher = t.Publisher,
                                       Version = t.Version,
                                       Year = t.Year
                                   }).ToList());
                }

                if (!string.IsNullOrEmpty(input.TxtSearch2.Trim()))
                {
                    var tempList = dbContext.FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK(input.LibraryId, input.LocationId,
                        input.SearchType2, input.DocumentType, input.TxtSearch2).ToList();

                    if (!flagForCondition1)
                    {
                        if (input.Condition1.Equals("and"))
                        {
                            var listBook = dbContext.FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID(input.LibraryId,
                                input.LocationId, input.DocumentType).ToList();
                            var resultList = GetBookList(input.TxtSearch2, input.SearchType2, listBook);

                            if (!string.IsNullOrEmpty(input.TxtSearch3.Trim()))
                            {
                                if (input.Condition2.Equals("and"))
                                {
                                    resultList = GetBookList(input.TxtSearch3, input.SearchType3, resultList);
                                    flagForCondition2 = true;
                                }
                            }

                            if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                            {
                                if (input.Condition3.Equals("and"))
                                {
                                    resultList = GetBookList(input.TxtSearch4, input.SearchType4, resultList);
                                    flagForCondition3 = true;
                                }
                            }

                            resultList = resultList.Distinct().ToList();

                            list.AddRange((from t in resultList
                                           select new FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result()
                                           {
                                               ID = t.ID,
                                               Title = t.Title,
                                               Author = t.Author,
                                               Publisher = t.Publisher,
                                               Version = t.Version,
                                               Year = t.Year
                                           }).ToList());
                        }
                    }

                    if (input.Condition1.Equals("or"))
                    {
                        list.AddRange(tempList);
                    }
                    else if (input.Condition1.Equals("not"))
                    {
                        deleteList.AddRange(tempList);
                    }
                }

                if (!string.IsNullOrEmpty(input.TxtSearch3.Trim()))
                {
                    var tempList = dbContext.FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK(input.LibraryId, input.LocationId,
                        input.SearchType3, input.DocumentType, input.TxtSearch3).ToList();

                    if (!flagForCondition2)
                    {
                        if (input.Condition2.Equals("and"))
                        {
                            var listBook = dbContext.FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID(input.LibraryId,
                                input.LocationId, input.DocumentType).ToList();
                            var resultList = GetBookList(input.TxtSearch3, input.SearchType3, listBook);

                            if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                            {
                                if (input.Condition3.Equals("and"))
                                {
                                    resultList = GetBookList(input.TxtSearch4, input.SearchType4, resultList);
                                    flagForCondition3 = true;
                                }
                            }

                            resultList = resultList.Distinct().ToList();

                            list.AddRange((from t in resultList
                                           select new FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result()
                                           {
                                               ID = t.ID,
                                               Title = t.Title,
                                               Author = t.Author,
                                               Publisher = t.Publisher,
                                               Version = t.Version,
                                               Year = t.Year
                                           }).ToList());
                        }
                    }

                    if (input.Condition2.Equals("or"))
                    {
                        list.AddRange(tempList);
                    }
                    else if (input.Condition2.Equals("not"))
                    {
                        deleteList.AddRange(tempList);
                    }
                }

                if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                {
                    var tempList = dbContext.FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK(input.LibraryId, input.LocationId,
                        input.SearchType4, input.DocumentType, input.TxtSearch4).ToList();

                    if (!flagForCondition3)
                    {
                        if (input.Condition3.Equals("and"))
                        {
                            list.AddRange(tempList);
                        }
                    }

                    if (input.Condition3.Equals("or"))
                    {
                        list.AddRange(tempList);
                    }
                    else if (input.Condition3.Equals("not"))
                    {
                        deleteList.AddRange(tempList);
                    }
                }

                list = list.Distinct().ToList();

                foreach (var item in list.Distinct().ToList())
                {
                    if (deleteList.Any(deleteItem => deleteItem.ID == item.ID))
                    {
                        list.Remove(item);
                    }
                }

                /*
                 * OrderBy = 1 : Sort by title
                 * OrderBy = 2 : Sort by author
                 * OrderBy = 3 : Sort by published year
                 * OrderBy = 4 : Sort by publisher
                 */
                if (input.OrderBy.Equals("1"))
                {
                    list = list.OrderBy(t => t.Title).ToList();
                }
                else if (input.OrderBy.Equals("2"))
                {
                    list = list.OrderBy(t => t.Author).ToList();
                }
                else if (input.OrderBy.Equals("3"))
                {
                    list = list.OrderBy(t => t.Year).ToList();
                }
                else
                {
                    list = list.OrderBy(t => t.Publisher).ToList();
                }
            }

            return list;
        }

        /// <summary>
        /// Get all copy number of book by itemID
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public static List<string> GetListCopyNumber(int itemID)
        {
            using (var dbContext = new OpacEntities())
            {
                var copyNum = (from g in dbContext.HOLDINGs
                               where g.ItemID == itemID
                               select g.CopyNumber).Take(18).ToList();

                return copyNum;
            }
        }

        /// <summary>
        /// Count number of book following by symbol
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static int CountTotalCopyNumberBySymbol(int itemID, string symbol)
        {
            using (var dbContext = new OpacEntities())
            {
                var counting = (from g in dbContext.HOLDINGs
                                join d in dbContext.HOLDING_LOCATION on g.LocationID equals d.ID
                                where g.ItemID == itemID && d.Symbol.Equals(symbol)
                                select g.CopyNumber).Count();

                return counting;
            }
        }

        /// <summary>
        /// Get title of item by itemID
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public string GetItemTitle(int itemID)
        {
            using (var dbContext = new OpacEntities())
            {
                var itemTitle = (from r in dbContext.FIELD200S
                                 where r.ItemID == itemID && r.FieldCode.Equals("245")
                                 select r.Content).FirstOrDefault();

                itemTitle = itemTitle?.Replace("$a", "").Replace("$b", " ")
                    .Replace("$c", " ").Replace("$n", " ");

                return itemTitle;
            }
        }

        /// <summary>
        /// Count number of book which is free to borrow, following by symbol
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static int CountTotalCopyNumberFreeBySymbol(int itemID, string symbol)
        {
            using (var dbContext = new OpacEntities())
            {
                var counting = (from g in dbContext.HOLDINGs
                                join d in dbContext.HOLDING_LOCATION on g.LocationID equals d.ID
                                where g.ItemID == itemID && d.Symbol.Equals(symbol) && g.InUsed == false
                                select g.CopyNumber).Count();

                return counting;
            }
        }

        /// <summary>
        /// Count number of result book after searching
        /// </summary>
        /// <param name="searchKeyword"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public int GetNumberResult(string searchKeyword, string option)
        {
            using (var dbContext = new OpacEntities())
            {
                searchKeyword = Regex.Replace(searchKeyword, @"\s+", " ").Trim();
                var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_Result>("FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK {0}, {1}",
                    new object[] { searchKeyword, option }).ToList();

                return list.Count;
            }
        }

        /// <summary>
        /// Count number of result book after searching by key word
        /// </summary>
        /// <param name="searchKeyword"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchBy"></param>
        /// <returns></returns>
        public int GetNumberResultByKeyWord(string searchKeyword, int page, int pageSize, int searchBy)
        {
            var listResult = new List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>();
            using (var dbContext = new OpacEntities())
            {
                if (searchBy == 1)
                {
                    var getItemID = dbContext.FPT_SP_GET_ITEMID_BY_KEYWORD(searchKeyword).ToList();
                    foreach (var item in getItemID)
                    {
                        var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>("FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD {0}",
                            new object[] { item }).ToList();

                        listResult.AddRange(list);
                    }

                    var uniqueIdList = listResult.Select(t => t.ID).Distinct().ToList();
                    var indexIdList = new List<int>();
                    var duplicatesIdList = listResult.Select((t, i) => new { Index = i, ItemID = t.ID }).GroupBy(g => g.ItemID)
                        .Where(g => g.Count() > 1);

                    foreach (var idList in duplicatesIdList)
                    {
                        int count = 0;
                        foreach (var item in idList)
                        {
                            count++;
                            if (count > 1) //Only find value have index > 0
                            {
                                foreach (var item1 in uniqueIdList) //Get index of duplicated value
                                {
                                    if (item.ItemID == item1)
                                    {
                                        indexIdList.Add(item.Index);
                                    }
                                }
                            }
                        }
                    }

                    //Get duplicated values
                    var duplicatedList = new List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>();
                    foreach (var index in indexIdList)
                    {
                        duplicatedList.Add(listResult[index]);
                    }

                    //Remove all duplicated values
                    foreach (var item in duplicatedList)
                    {
                        listResult.Remove(item);
                    }
                }
                else if (searchBy == 2)
                {
                    var getItemID = dbContext.FPT_SP_OPAC_GET_ITEMID_BY_DDC(searchKeyword).ToList();
                    foreach (var item in getItemID)
                    {
                        var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>("FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD {0}",
                            new object[] { item }).ToList();

                        listResult.AddRange(list);
                    }

                    var uniqueIdList = listResult.Select(t => t.ID).Distinct().ToList();
                    var indexIdList = new List<int>();
                    var duplicatesIdList = listResult.Select((t, i) => new { Index = i, ItemID = t.ID }).GroupBy(g => g.ItemID)
                        .Where(g => g.Count() > 1);

                    foreach (var idList in duplicatesIdList)
                    {
                        int count = 0;
                        foreach (var item in idList)
                        {
                            count++;
                            if (count > 1) //Only find value have index > 0
                            {
                                foreach (var item1 in uniqueIdList) //Get index of duplicated value
                                {
                                    if (item.ItemID == item1)
                                    {
                                        indexIdList.Add(item.Index);
                                    }
                                }
                            }
                        }
                    }

                    //Get duplicated values
                    var duplicatedList = new List<FPT_SP_OPAC_GET_SEARCHED_INFO_BOOK_BY_KEYWORD_Result>();
                    foreach (var index in indexIdList)
                    {
                        duplicatedList.Add(listResult[index]);
                    }

                    //Remove all duplicated values
                    foreach (var item in duplicatedList)
                    {
                        listResult.Remove(item);
                    }
                }

                return listResult.ToPagedList(page, pageSize).TotalItemCount;
            }
        }

        /// <summary>
        /// Count number of result advanced search
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int GetNumberResultByAdvancedSearch(AdvancedSearch input)
        {
            var list = new List<FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result>();
            var deleteList = new List<FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result>();
            bool flagForCondition1 = false;
            bool flagForCondition2 = false;
            bool flagForCondition3 = false;

            using (var dbContext = new OpacEntities())
            {
                if (!string.IsNullOrEmpty(input.TxtSearch1.Trim()))
                {
                    var listBook = dbContext.FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID(input.LibraryId,
                        input.LocationId, input.DocumentType).ToList();
                    var tempList = GetBookList(input.TxtSearch1, input.SearchType1, listBook);

                    if (!string.IsNullOrEmpty(input.TxtSearch2.Trim()))
                    {
                        if (input.Condition1.Equals("and"))
                        {
                            tempList = GetBookList(input.TxtSearch2, input.SearchType2, tempList);
                            flagForCondition1 = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(input.TxtSearch3.Trim()))
                    {
                        if (input.Condition2.Equals("and"))
                        {
                            tempList = GetBookList(input.TxtSearch3, input.SearchType3, tempList);
                            flagForCondition2 = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                    {
                        if (input.Condition3.Equals("and"))
                        {
                            tempList = GetBookList(input.TxtSearch4, input.SearchType4, tempList);
                            flagForCondition3 = true;
                        }
                    }

                    tempList = tempList.Distinct().ToList();

                    list.AddRange((from t in tempList
                                   select new FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result()
                                   {
                                       ID = t.ID,
                                       Title = t.Title,
                                       Author = t.Author,
                                       Publisher = t.Publisher,
                                       Version = t.Version,
                                       Year = t.Year
                                   }).ToList());
                }

                if (!string.IsNullOrEmpty(input.TxtSearch2.Trim()))
                {
                    var tempList = dbContext.FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK(input.LibraryId, input.LocationId,
                        input.SearchType2, input.DocumentType, input.TxtSearch2).ToList();

                    if (!flagForCondition1)
                    {
                        if (input.Condition1.Equals("and"))
                        {
                            var listBook = dbContext.FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID(input.LibraryId,
                                input.LocationId, input.DocumentType).ToList();
                            var resultList = GetBookList(input.TxtSearch2, input.SearchType2, listBook);

                            if (!string.IsNullOrEmpty(input.TxtSearch3.Trim()))
                            {
                                if (input.Condition2.Equals("and"))
                                {
                                    resultList = GetBookList(input.TxtSearch3, input.SearchType3, resultList);
                                    flagForCondition2 = true;
                                }
                            }

                            if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                            {
                                if (input.Condition3.Equals("and"))
                                {
                                    resultList = GetBookList(input.TxtSearch4, input.SearchType4, resultList);
                                    flagForCondition3 = true;
                                }
                            }

                            resultList = resultList.Distinct().ToList();

                            list.AddRange((from t in resultList
                                           select new FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result()
                                           {
                                               ID = t.ID,
                                               Title = t.Title,
                                               Author = t.Author,
                                               Publisher = t.Publisher,
                                               Version = t.Version,
                                               Year = t.Year
                                           }).ToList());
                        }
                    }

                    if (input.Condition1.Equals("or"))
                    {
                        list.AddRange(tempList);
                    }
                    else if (input.Condition1.Equals("not"))
                    {
                        deleteList.AddRange(tempList);
                    }
                }

                if (!string.IsNullOrEmpty(input.TxtSearch3.Trim()))
                {
                    var tempList = dbContext.FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK(input.LibraryId, input.LocationId,
                        input.SearchType3, input.DocumentType, input.TxtSearch3).ToList();

                    if (!flagForCondition2)
                    {
                        if (input.Condition2.Equals("and"))
                        {
                            var listBook = dbContext.FPT_SP_OPAC_GET_BOOK_BY_LIBID_AND_LOCATIONID(input.LibraryId,
                                input.LocationId, input.DocumentType).ToList();
                            var resultList = GetBookList(input.TxtSearch3, input.SearchType3, listBook);

                            if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                            {
                                if (input.Condition3.Equals("and"))
                                {
                                    resultList = GetBookList(input.TxtSearch4, input.SearchType4, resultList);
                                    flagForCondition3 = true;
                                }
                            }

                            resultList = resultList.Distinct().ToList();

                            list.AddRange((from t in resultList
                                           select new FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK_Result()
                                           {
                                               ID = t.ID,
                                               Title = t.Title,
                                               Author = t.Author,
                                               Publisher = t.Publisher,
                                               Version = t.Version,
                                               Year = t.Year
                                           }).ToList());
                        }
                    }

                    if (input.Condition2.Equals("or"))
                    {
                        list.AddRange(tempList);
                    }
                    else if (input.Condition2.Equals("not"))
                    {
                        deleteList.AddRange(tempList);
                    }
                }

                if (!string.IsNullOrEmpty(input.TxtSearch4.Trim()))
                {
                    var tempList = dbContext.FPT_SP_OPAC_GET_ADVANCED_SEARCHED_INFO_BOOK(input.LibraryId, input.LocationId,
                        input.SearchType4, input.DocumentType, input.TxtSearch4).ToList();

                    if (!flagForCondition3)
                    {
                        if (input.Condition3.Equals("and"))
                        {
                            list.AddRange(tempList);
                        }
                    }

                    if (input.Condition3.Equals("or"))
                    {
                        list.AddRange(tempList);
                    }
                    else if (input.Condition3.Equals("not"))
                    {
                        deleteList.AddRange(tempList);
                    }
                }

                list = list.Distinct().ToList();

                foreach (var item in list.Distinct().ToList())
                {
                    if (deleteList.Any(deleteItem => deleteItem.ID == item.ID))
                    {
                        list.Remove(item);
                    }
                }
            }

            return list.Count;
        }

        /// <summary>
        /// Get detail information of book and display by MARC
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="isAuthority"></param>
        /// <returns></returns>
        public List<SP_CATA_GET_CONTENTS_OF_ITEMS_Result> SP_CATA_GET_CONTENTS_OF_ITEMS_LIST(int itemID, int isAuthority)
        {
            using (var dbContext = new OpacEntities())
            {
                var list = dbContext.Database.SqlQuery<SP_CATA_GET_CONTENTS_OF_ITEMS_Result>("SP_CATA_GET_CONTENTS_OF_ITEMS {0}, {1}",
                    new object[] { itemID, isAuthority }).ToList();

                return list;
            }
        }

        /// <summary>
        /// Get related term of a book
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public List<FPT_SP_OPAC_GET_RELATED_TERMS_Result> FPT_SP_OPAC_GET_RELATED_TERMS_LIST(int itemID)
        {
            using (var dbContext = new OpacEntities())
            {
                var list = dbContext.Database.SqlQuery<FPT_SP_OPAC_GET_RELATED_TERMS_Result>("FPT_SP_OPAC_GET_RELATED_TERMS {0}",
                    new object[] { itemID }).ToList();

                return list;
            }
        }

        /// <summary>
        /// Get all copy number of book
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public List<FPT_SP_GET_CODE_AND_SYMBOL_BY_ITEMID_Result> GetInforCopyNumberList(int itemID)
        {
            using (var dbContext = new OpacEntities())
            {
                var list = dbContext.Database.SqlQuery<FPT_SP_GET_CODE_AND_SYMBOL_BY_ITEMID_Result>("FPT_SP_GET_CODE_AND_SYMBOL_BY_ITEMID {0}",
                    new object[] { itemID }).ToList();

                return list;
            }
        }

        /// <summary>
        /// Get detal book with status
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static List<FPT_SP_GET_DETAIL_BOOK_WITH_STATUS_Result> GetDetailBookWithStatus(int itemID, string code)
        {
            using (var dbContext = new OpacEntities())
            {
                var list = dbContext.Database.SqlQuery<FPT_SP_GET_DETAIL_BOOK_WITH_STATUS_Result>("FPT_SP_GET_DETAIL_BOOK_WITH_STATUS {0}, {1}",
                    new object[] { itemID, code }).ToList();

                return list;
            }
        }

        /// <summary>
        /// Get full information of book after searching (Lấy thông tin sách hiển thị đầy đủ)
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public FullInforBook GetFullInforBook(int itemID)
        {
            StringBuilder getDocumentType = new StringBuilder("");
            StringBuilder getISBN = new StringBuilder("");
            StringBuilder getLanguageCode = new StringBuilder("");
            StringBuilder getPublishing = new StringBuilder("");
            StringBuilder getPublishingYear = new StringBuilder("");
            StringBuilder getPhysicDescription = new StringBuilder("");
            StringBuilder getBrief = new StringBuilder("");
            string[] specialCharacterList = { "$a", "$b", "$c", "$p", "$e", "$n" };

            using (var dbContext = new OpacEntities())
            {
                var getISBNTemp = dbContext.FPT_SP_GET_ISBN_ITEM(itemID).ToList();
                foreach (var item in getISBNTemp)
                {
                    getISBN.Append(item + " ");
                }

                var getLanguageCodeTemp = dbContext.FPT_SP_GET_LANGUAGE_CODE_ITEM(itemID).ToList();
                foreach (var item in getLanguageCodeTemp)
                {
                    getLanguageCode.Append(item);
                }

                var getPublishingTemp = dbContext.FPT_SP_GET_PUBLISH_INFO_ITEM(itemID).ToList();
                foreach (var item in getPublishingTemp)
                {
                    getPublishing.Append(item + " ");
                }

                string[] temp = getPublishing.ToString().Split(new[] { "$c" }, StringSplitOptions.None);
                if (temp.Length == 1)
                {
                    getPublishing = new StringBuilder(temp[0]);
                }
                else
                {
                    getPublishing = new StringBuilder(temp[0]);
                    getPublishingYear.Append(temp[1]);
                }

                var getPhysicDescriptionTemp = dbContext.FPT_SP_GET_PHYSICAL_INFO_ITEM(itemID).ToList();
                foreach (var item in getPhysicDescriptionTemp)
                {
                    getPhysicDescription.Append(item + " ");
                }

                foreach (var item in specialCharacterList)
                {
                    getISBN.Replace(item, "");
                    getLanguageCode.Replace(item, "");
                    getPublishing.Replace(item, "");
                    getPublishingYear.Replace(item, "");
                    getPhysicDescription.Replace(item, "");
                }
            }

            FullInforBook fullBookInformation = new FullInforBook
            {
                ISBN = getISBN.ToString().Trim(),
                LanguageCode = getLanguageCode.ToString().Trim(),
                Publishing = getPublishing.ToString().Trim(),
                PublishingYear = getPublishingYear.ToString().Trim(),
                PhysicDescription = getPhysicDescription.ToString().Trim()
            };

            return fullBookInformation;
        }

        /// <summary>
        /// Get total of book
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public int GetTotalBook(int itemID)
        {
            using (var dbContext = new OpacEntities())
            {
                var totalBook = (from total in dbContext.HOLDINGs
                                 where total.ItemID == itemID
                                 select total).Count();

                return totalBook;
            }
        }

        /// <summary>
        /// Get total of book which is not using yet
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public int GetFreeBook(int itemID)
        {
            using (var dbContext = new OpacEntities())
            {
                var freeBook = (from f in dbContext.HOLDINGs
                                where f.ItemID == itemID && f.InUsed == false
                                select f).Count();

                return freeBook;
            }
        }

        /// <summary>
        /// Get number of book preparing to borrow by patron
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public int GetOnHoldingBook(int itemID)
        {
            using (var dbContext = new OpacEntities())
            {
                var holdingBook = (from h in dbContext.CIR_HOLDING
                                   where h.ItemID == itemID && 
                                         (h.InTurn == true && h.CheckMail || 
                                         h.InTurn == true && !h.CheckMail ||
                                         h.InTurn == false && !h.CheckMail)
                                   select h).Count();

                return holdingBook;
            }
        }

        /// <summary>
        /// Get list of document
        /// </summary>
        /// <returns></returns>
        public List<SP_OPAC_GET_DIC_ITEM_TYPE_Result> GetDocumentType()
        {
            using (var dbContext = new OpacEntities())
            {
                return dbContext.SP_OPAC_GET_DIC_ITEM_TYPE().ToList();
            }
        }

        /// <summary>
        /// Get list of library
        /// </summary>
        /// <returns></returns>
        public List<SP_GET_LIBRARY_Result> GetLibrary()
        {
            using (var dbContext = new OpacEntities())
            {
                return dbContext.SP_GET_LIBRARY().ToList();
            }
        }

        /// <summary>
        /// Get location
        /// </summary>
        /// <param name="libId"></param>
        /// <returns></returns>
        public List<Location> GetLocation(int libId)
        {
            using (var dbContext = new OpacEntities())
            {
                var list = new List<Location>();
                var locationList = dbContext.FPT_SP_OPAC_GET_LOCATION().Where(t => t.LibID == libId).ToList();
                foreach (var item in locationList)
                {
                    Location location = new Location()
                    {
                        ID = item.ID,
                        LibID = item.LibID,
                        SymbolAndCodeLoc = item.Symbol + " (" + item.CodeLoc + ")",
                        MaxNumber = item.MaxNumber,
                        Status = item.Status
                    };
                    list.Add(location);
                }

                return list;
            }
        }

        /// <summary>
        /// Get summary field
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public string GetSummary(int itemID)
        {
            using (var dbContext = new OpacEntities())
            {
                var summary = "";
                try
                {
                    summary = (from s in dbContext.FIELD500S
                               where s.FieldCode.Equals("520") && s.ItemID == itemID
                               select s.Content).FirstOrDefault();

                    summary = summary.Replace("$a", "");
                }
                catch (NullReferenceException)
                {
                    summary = "";
                }

                return summary;
            }
        }

        /// <summary>
        /// Get record type
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public string GetRecordType(int itemID)
        {
            using (var dbContext = new OpacEntities())
            {
                return (from t in dbContext.ITEMs
                        join q in dbContext.CAT_DIC_RECORDTYPE on t.RecordType equals q.Code
                        where t.ID == itemID
                        select q.Description).FirstOrDefault();
            }
        }

        /// <summary>
        /// Get search type list for advanced search
        /// </summary>
        /// <returns></returns>
        public List<CAT_DIC_LIST> GetSearchTypeDicList()
        {
            using (var dbContext = new OpacEntities())
            {
                return (from t in dbContext.CAT_DIC_LIST select t).ToList();
            }
        }

        /// <summary>
        /// Add information of patron who registered to borrow book
        /// </summary>
        /// <param name="holding"></param>
        public void RegisterToBorrow(CIR_HOLDING holding)
        {
            using (var dbContext = new OpacEntities())
            {
                dbContext.CIR_HOLDING.Add(holding);
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Check book is in FPT Hoa Lac Library
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static bool IsBookInFptHoaLacLibrary(int itemId)
        {
            using (var dbContext = new OpacEntities())
            {
                var book = dbContext.HOLDINGs.Where(t => t.ItemID == itemId).ToList();

                return book.Any(t => t.LibID == 81);
            }
        }
    }
}