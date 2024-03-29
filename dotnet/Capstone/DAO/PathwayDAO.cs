﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.DAO.Interfaces;
using Capstone.Models;

namespace Capstone.DAO
{
    public class PathwayDAO : IPathwayDAO
    {
        private string connectionString;

        private string sqlGetPathwayResponse = "SELECT pathway.pathway_id, pathway.response FROM pathway " +
            "JOIN pathway_keywords pk ON pk.pathway_id = pathway.pathway_id " +
            "JOIN keywords k ON k.keyword_id = pk.keyword_id " +
            "WHERE @keyword like '%' + keyword + '%'";


        public PathwayDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public BotMessage GetPathwayResponse(UserMessage message)
        {
            BotMessage response = new BotMessage();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlGetPathwayResponse, conn);
                    cmd.Parameters.AddWithValue("@keyword", message.Message);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        response = ReaderToPathwayResponse(reader);

                    }

                }

            }
            catch (Exception ex)
            {
                response = new BotMessage();
            }
            return response;
        }
        private BotMessage ReaderToPathwayResponse(SqlDataReader reader)
        {
            BotMessage pathwayResponse = new BotMessage();
            pathwayResponse.Id = Convert.ToInt32(reader["pathway_id"]);
            pathwayResponse.Response = Convert.ToString(reader["response"]);
        
            return pathwayResponse;

        }
       
    }
}
