using Dapper;
using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces.Admin;
using Models.Admin;
using SmartParking.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Lab_Mvc.Repositries.Admin
{
    public class PortfolioProjectRepository : DapperRepositoryBase, IPortfolioProjectRepository
    {
        public PortfolioProjectRepository(DapperContext dapperContext) : base(dapperContext)
        {
        }

        public async Task<IEnumerable<PortfolioProject>> GetAllProjectsAsync(bool onlyActive = false)
        {
            try
            {
                var query = @"
                    SELECT * FROM PortfolioProjects 
                    WHERE (@OnlyActive = 0 OR IsActive = 1)
                    ORDER BY SrNo ASC;
                ";

                using (var con = CreateConnection())
                {
                    return await con.QueryAsync<PortfolioProject>(query, new { OnlyActive = onlyActive });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PortfolioProject> GetProjectByIdAsync(int id)
        {
            try
            {
                var query = "SELECT * FROM PortfolioProjects WHERE ProjectId = @ProjectId;";

                using (var con = CreateConnection())
                {
                    return await con.QuerySingleOrDefaultAsync<PortfolioProject>(query, new { ProjectId = id });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> AddProjectAsync(PortfolioProject project)
        {
            try
            {
                var query = @"
                    INSERT INTO PortfolioProjects (
                        SrNo, ProjectName, ProjectDescription, Technologies, CodeLink, LiveDemoLink, 
                        ApkFile, DesktopFile, Image1, Image2, Image3, Image4, Category, IsActive, 
                        CreatedDate, ModifiedDate
                    )
                    VALUES (
                        @SrNo, @ProjectName, @ProjectDescription, @Technologies, @CodeLink, @LiveDemoLink, 
                        @ApkFile, @DesktopFile, @Image1, @Image2, @Image3, @Image4, @Category, @IsActive, 
                        GETDATE(), GETDATE()
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);
                ";

                using (var con = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@SrNo", project.SrNo);
                    parameters.Add("@ProjectName", project.ProjectName ?? "");
                    parameters.Add("@ProjectDescription", project.ProjectDescription ?? "");
                    parameters.Add("@Technologies", project.Technologies ?? "");
                    parameters.Add("@CodeLink", project.CodeLink ?? "");
                    parameters.Add("@LiveDemoLink", project.LiveDemoLink ?? "");
                    parameters.Add("@ApkFile", project.ApkFile ?? "");
                    parameters.Add("@DesktopFile", project.DesktopFile ?? "");
                    parameters.Add("@Image1", project.Image1 ?? "");
                    parameters.Add("@Image2", project.Image2 ?? "");
                    parameters.Add("@Image3", project.Image3 ?? "");
                    parameters.Add("@Image4", project.Image4 ?? "");
                    parameters.Add("@Category", project.Category ?? "webApp");
                    parameters.Add("@IsActive", project.IsActive);

                    return await con.QuerySingleAsync<int>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> UpdateProjectAsync(PortfolioProject project)
        {
            try
            {
                var query = @"
                    UPDATE PortfolioProjects 
                    SET SrNo = @SrNo,
                        ProjectName = @ProjectName,
                        ProjectDescription = @ProjectDescription,
                        Technologies = @Technologies,
                        CodeLink = @CodeLink,
                        LiveDemoLink = @LiveDemoLink,
                        ApkFile = @ApkFile,
                        DesktopFile = @DesktopFile,
                        Image1 = @Image1,
                        Image2 = @Image2,
                        Image3 = @Image3,
                        Image4 = @Image4,
                        Category = @Category,
                        IsActive = @IsActive,
                        ModifiedDate = GETDATE()
                    WHERE ProjectId = @ProjectId;
                ";

                using (var con = CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ProjectId", project.ProjectId);
                    parameters.Add("@SrNo", project.SrNo);
                    parameters.Add("@ProjectName", project.ProjectName ?? "");
                    parameters.Add("@ProjectDescription", project.ProjectDescription ?? "");
                    parameters.Add("@Technologies", project.Technologies ?? "");
                    parameters.Add("@CodeLink", project.CodeLink ?? "");
                    parameters.Add("@LiveDemoLink", project.LiveDemoLink ?? "");
                    parameters.Add("@ApkFile", project.ApkFile ?? "");
                    parameters.Add("@DesktopFile", project.DesktopFile ?? "");
                    parameters.Add("@Image1", project.Image1 ?? "");
                    parameters.Add("@Image2", project.Image2 ?? "");
                    parameters.Add("@Image3", project.Image3 ?? "");
                    parameters.Add("@Image4", project.Image4 ?? "");
                    parameters.Add("@Category", project.Category ?? "webApp");
                    parameters.Add("@IsActive", project.IsActive);

                    return await con.ExecuteAsync(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> DeleteProjectAsync(int id)
        {
            try
            {
                var query = "UPDATE PortfolioProjects SET IsActive = 0, ModifiedDate = GETDATE() WHERE ProjectId = @ProjectId;";

                using (var con = CreateConnection())
                {
                    return await con.ExecuteAsync(query, new { ProjectId = id });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
