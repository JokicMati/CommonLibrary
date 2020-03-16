using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Common.EF
{
    public static class DbOperateExtensions
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public static IQueryable<TModel> Get<TModel>(this DbContext db, Expression<Func<TModel, bool>> where) where TModel : class
        {
            return db.Set<TModel>().Where(where);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public static IQueryable<TModel> GetAsNoTracking<TModel>(this DbContext db, Expression<Func<TModel, bool>> where) where TModel : class
        {
            return db.Set<TModel>().AsNoTracking().Where(where);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TOrderResult"></typeparam>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">每页数据行数</param>
        /// <param name="isAsc">正序</param>
        /// <param name="count">查询总行数</param>
        /// <param name="where">查询条件</param>
        /// <param name="order">排序</param>
        /// <returns></returns>
        public static IQueryable<TModel> GetPage<TModel, TOrderResult>(this DbContext db, int pageNumber, int pageSize, bool isAsc, out int count, Expression<Func<TModel, bool>> where, Expression<Func<TModel, TOrderResult>> order) where TModel : class
        {
            var lsRet = db.Set<TModel>().Where(where);

            count = lsRet.Count();

            if (isAsc == true)
            {
                lsRet = lsRet.OrderBy(order);
            }
            else
            {
                lsRet = lsRet.OrderByDescending(order);
            }

            lsRet = lsRet.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return lsRet.AsQueryable();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Add<TModel>(this DbContext db, TModel model) where TModel : class
        {
            db.Set<TModel>().Attach(model);
            db.Entry<TModel>(model).State = EntityState.Added;
            return db.SaveChanges();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="where">删除条件</param>
        /// <returns></returns>
        public static int Delete<TModel>(this DbContext db, Expression<Func<TModel, bool>> where) where TModel : class
        {
            var finds = where == null ? db.Set<TModel>() : db.Set<TModel>().Where(where);
            db.Set<TModel>().RemoveRange(finds);
            return db.SaveChanges();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update<TModel>(this DbContext db, TModel model) where TModel : class
        {
            db.Set<TModel>().Attach(model);
            db.Entry<TModel>(model).State = EntityState.Modified;
            return db.SaveChanges();
        }


    }
}
