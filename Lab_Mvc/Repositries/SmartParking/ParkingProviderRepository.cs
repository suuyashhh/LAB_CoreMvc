using Dapper;
using Models;
using Lab_Mvc.Contest;
using SmartParking.Interfaces;

namespace SmartParking.Repositories
{
    public class ParkingProviderRepository : DapperRepositoryBase, IParkingProvider
    {
        public ParkingProviderRepository(DapperContext context) : base(context)
        {
        }

        public async Task<string> SaveParkingLocation(DTOParkingProvider req)
        {
            try
            {
                string query;
                if (req.Unique_Id > 0)
                {
                    query = @"
                        UPDATE SMARTPARKING_parking_spot 
                        SET VehicalType = @VehicalType, 
                            LatitudeLangitude = @LatitudeLangitude, 
                            FullAddress = @FullAddress, 
                            price = @price, 
                            contact = @contact, 
                            img1 = ISNULL(@img1, img1), 
                            img2 = ISNULL(@img2, img2), 
                            img3 = ISNULL(@img3, img3), 
                            img4 = ISNULL(@img4, img4) 
                        WHERE Unique_Id = @Unique_Id";
                }
                else
                {
                    query = @"
                        INSERT INTO SMARTPARKING_parking_spot 
                        (UserId, VehicalType, LatitudeLangitude, FullAddress, price, contact, img1, img2, img3, img4) 
                        VALUES 
                        (@UserId, @VehicalType, @LatitudeLangitude, @FullAddress, @price, @contact, @img1, @img2, @img3, @img4)";
                }

                using (var con = CreateConnection())
                {
                    await con.ExecuteAsync(query, req);
                    return req.Unique_Id > 0 ? "Parking location updated successfully." : "Parking location saved successfully.";
                }
            }

            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }

        public async Task<List<DTOParkingProvider>> GetParkingLocationsByUser(int userId)
        {
            try
            {
                var query = "SELECT * FROM SMARTPARKING_parking_spot WHERE UserId = @UserId";
                using (var con = CreateConnection())
                {
                    var result = await con.QueryAsync<DTOParkingProvider>(query, new { UserId = userId });
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                return new List<DTOParkingProvider>();
            }
        }

        public async Task<List<DTOParkingProvider>> GetAllParkingLocations()
        {
            try
            {
                var query = "SELECT * FROM SMARTPARKING_parking_spot";
                using (var con = CreateConnection())
                {
                    var result = await con.QueryAsync<DTOParkingProvider>(query);
                    return result.ToList();
                }
            }
            catch (Exception)
            {
                return new List<DTOParkingProvider>();
            }
        }

        public async Task<string> DeleteParkingLocation(int uniqueId)
        {
            try
            {
                var query = "DELETE FROM SMARTPARKING_parking_spot WHERE Unique_Id = @UniqueId";
                using (var con = CreateConnection())
                {
                    await con.ExecuteAsync(query, new { UniqueId = uniqueId });
                    return "Parking location deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
    }
}



