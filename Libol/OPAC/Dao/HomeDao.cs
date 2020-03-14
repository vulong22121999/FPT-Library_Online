using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OPAC.Models;

namespace OPAC.Dao
{
    public class HomeDao
    {
        /// <summary>
        /// Get code name of library
        /// </summary>
        /// <param name="libId"></param>
        /// <returns></returns>
        public static string GetLibraryCode(int libId)
        {
            using (var dbContext = new OpacEntities())
            {
                return dbContext.HOLDING_LIBRARY.Where(t => t.ID == libId).ToList().Select(t => t.Code)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Get type of notice
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static string GetTypeNotice(int typeId)
        {
            using (var dbContext = new OpacEntities())
            {
                return dbContext.TYPE_NOTICE.Where(t => t.ID == typeId).ToList().Select(t => t.TypeNotice)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Get name of library
        /// </summary>
        /// <param name="libId"></param>
        /// <returns></returns>
        public static string GetLibraryName(int libId)
        {
            using (var dbContext = new OpacEntities())
            {
                return dbContext.HOLDING_LIBRARY.Where(t => t.ID == libId).ToList().Select(t => t.Name)
                    .FirstOrDefault();
            }
        }
    }
}