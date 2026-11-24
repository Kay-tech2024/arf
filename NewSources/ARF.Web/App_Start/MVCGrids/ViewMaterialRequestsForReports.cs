using ARF.Core;
using ARF.Data;
using ARF.Data.DbModels;
using ARF.Web.App_Start.Code;
using ARF.Web.Models;
using ARF.Web.Resources;
using MVCGrid.Models;
using MVCGrid.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.App_Start.MVCGrids
{
    public class ViewMaterialRequestsForReports
    {
        public static void RegisterGrids()
        {

            GridDefaults defaultSet1 = new GridDefaults()
            {
                Paging = true,
                ItemsPerPage = 20,
                Sorting = true,
                NoResultsMessage = LangHelpers.Instance["no_results"]
            };

            ColumnDefaults colDefaults = new ColumnDefaults()
            {
                EnableSorting = true
            };


            MVCGridDefinitionTable.Add("ViewMaterialRequestsForReports", new MVCGridBuilder<MaterialRequestsByCollege>(colDefaults)
                    .WithAuthorizationType(AuthorizationType.AllowAnonymous)
                    .AddColumns(cols =>
                    {

                        cols.Add("Id")
                            .WithColumnName("الرقم")
                            .WithValueExpression(b => b.Id.ToString());
                        

                        cols.Add("Name")
                           .WithColumnName("الاسم")
                           .WithValueExpression(b => b.CollegeNameArabic);

                        
                        cols.Add("Status")
                           .WithValueExpression(b => LangHelpers.Instance[((RequestStatus)b.Status).ToString()]);

                    })
                    .WithProcessingMessage("فضلًا انتظر..")
                    .WithNextButtonCaption("التالي").WithNoResultsMessage("لا توجد نتائج")
                    .WithPreviousButtonCaption("السابق")
                    .WithSummaryMessage("عرض  {0} الى {1} من {2} مدخلات")
                    .WithNoResultsMessage("لا توجد نتائج")
                    .WithPaging(true, itemsPerPage: 20, allowChangePageSize: true, maxItemsPerPage: 100)
                    .WithSorting(true, "Id")
                    .WithAdditionalQueryOptionNames("Search")
                    .WithAdditionalSetting("RenderLoadingDiv", false)
                    .WithRetrieveDataMethod((context) =>
                    {
                        var options = context.QueryOptions;
                        string globalSearch = options.GetAdditionalQueryOptionString("Search");

                        var collegeId = (short)0;
                        var statusNo = (short)0;


                        if (context.CurrentHttpContext.Request["RequestStatus"] != null && short.TryParse(context.CurrentHttpContext.Request["RequestStatus"], out statusNo))
                        {
                        }


                        if (context.CurrentHttpContext.Request["CollegeId"] != null && short.TryParse(context.CurrentHttpContext.Request["CollegeId"], out collegeId))
                        {
                        }

                        var query = (IQueryable<MaterialRequestsByCollege>)null;

                        query = Utils.db.FuncMaterialRequestsByCollege(collegeId, statusNo)
                                .AsQueryable()
                                .Select(c => new MaterialRequestsByCollege
                                {
                                    Id = c.Id,
                                    Created = c.Created,
                                    Updated = c.Updated,
                                    CreatedBy = c.CreatedBy,
                                    UpdatedBy = c.UpdatedBy,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    FieldType = c.FieldType,
                                    SecondName = c.SecondName,
                                    Status = c.Status,
                                    ThirdName = c.ThirdName,
                                    Title = c.Title,
                                    Type = c.Type,
                                    ProjectStartDate = c.ProjectStartDate,
                                    ProjectEndDate = c.ProjectEndDate,
                                    Amount = c.Amount,
                                    CollegeNameArabic = c.CollegeNameArabic,
                                    CollegeNameEnglish = c.CollegeNameEnglish
                                    
                                });


                        if (options.SortColumnName != null)
                        {
                            switch (options.SortColumnName)
                            {
                                case "Id":
                                    query = options.SortDirection == MVCGrid.Models.SortDirection.Asc ? query.OrderBy(b => b.Id) : query.OrderByDescending(b => b.Id);
                                    break;

                                case "Name":
                                    query = options.SortDirection == MVCGrid.Models.SortDirection.Asc ? query.OrderBy(b => b.FirstName).ThenBy(b => b.LastName) : query.OrderByDescending(b => b.LastName).ThenByDescending(b => b.FirstName);
                                    break;

                                case "Title":
                                    query = options.SortDirection == MVCGrid.Models.SortDirection.Asc ? query.OrderBy(b => b.Title) : query.OrderByDescending(b => b.Title);
                                    break;
                                case "FieldType":
                                    query = options.SortDirection == MVCGrid.Models.SortDirection.Asc ? query.OrderBy(b => b.FieldType) : query.OrderByDescending(b => b.FieldType);
                                    break;
                                case "Status":
                                    query = options.SortDirection == MVCGrid.Models.SortDirection.Asc ? query.OrderBy(b => b.Status) : query.OrderByDescending(b => b.Status);
                                    break;
                                case "Updated":
                                default:
                                    query = options.SortDirection == MVCGrid.Models.SortDirection.Asc ? query.OrderBy(b => b.Updated) : query.OrderByDescending(b => b.Updated);
                                    break;

                            }
                        }
                        else
                        {
                            query = query.OrderByDescending(c => c.Updated);
                        }

                        if (options.ItemsPerPage == null || options.ItemsPerPage < 1)
                        {
                            options.ItemsPerPage = 20;
                        }

                        if (options.PageIndex == null || options.PageIndex < 0)
                        {
                            options.PageIndex = 0;
                        }

                        var total = query.Count();
                        var data = query.Skip(options.PageIndex.Value * options.ItemsPerPage.Value).Take(options.ItemsPerPage.Value);

                        // Query your data here. Obey Ordering, paging and filtering paramters given in the context.QueryOptions.
                        // Use Entity Framwork, a module from your IoC Container, or any other method.
                        // Return QueryResult object containing IEnumerable<YouModelItem>
                        return new QueryResult<MaterialRequestsByCollege>()
                        {
                            Items = data,
                            TotalRecords = total
                        };
                    })
            );
            
        }

    }
}

