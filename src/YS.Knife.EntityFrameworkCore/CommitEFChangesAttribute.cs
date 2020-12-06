﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife.Aop;
using YS.Knife.EntityFrameworkCore;

namespace YS.Knife.EntityFrameworkCore
{
    public class CommitEFChangesAttribute : BaseAopAttribute
    {
        public CommitEFChangesAttribute()
        {
            this.Order = 10000;
        }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var services = context?.ServiceProvider.GetService<IEnumerable<ICommitEFChangesContext>>().ToList(); ;
            if (services.Count == 0)
            {
                await next?.Invoke(context);
            }
            else if (services.Count == 1)
            {
                await next?.Invoke(context);
                var dbContext = services.First().DbContext;
                if (dbContext.ChangeTracker.HasChanges())
                {
                    await services.First().DbContext.SaveChangesAsync();
                }
            }
            else
            {
                await next?.Invoke(context);
                var allChangedContext = services.Where(p => p.DbContext.ChangeTracker.HasChanges())
                                .Select(p => p.DbContext)
                                .Distinct()
                                .ToList();
                if (allChangedContext.Count > 1)
                {
                    var tran = await allChangedContext.First().Database.BeginTransactionAsync();
                    var dbTran = tran.GetDbTransaction();
                    for (int i = 1; i < allChangedContext.Count; i++)
                    {
                        allChangedContext[i].Database.UseTransaction(dbTran);
                    }
                    try
                    {
                        foreach (var efContext in allChangedContext)
                        {
                            await efContext.SaveChangesAsync();
                        }
                        dbTran.Commit();
                    }
                    catch (Exception)
                    {
                        dbTran.Rollback();
                        throw;
                    }
                }
                else
                {
                    await allChangedContext.First().SaveChangesAsync();
                }
            }
        }
    }


}
