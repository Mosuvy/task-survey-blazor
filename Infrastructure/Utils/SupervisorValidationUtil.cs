using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskSurvey.Infrastructure.Data;

namespace TaskSurvey.Infrastructure.Utils
{
    public class SupervisorValidationUtil
    {
        /// <summary>
        /// Check if setting a supervisor would create a circular reference in the hierarchy
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="userId">The user who will get a new supervisor</param>
        /// <param name="proposedSupervisorId">The proposed supervisor ID</param>
        /// <returns>True if circular reference would be created, False otherwise</returns>
        public static async Task<bool> WouldCreateCircularReference(
            AppDbContext context, 
            string userId, 
            string proposedSupervisorId)
        {
            if (userId == proposedSupervisorId)
            {
                return true;
            }

            return await IsSubordinateOf(context, proposedSupervisorId, userId);
        }

        /// <summary>
        /// Check if targetUserId is a subordinate of supervisorId (directly or indirectly)
        /// </summary>
        private static async Task<bool> IsSubordinateOf(
            AppDbContext context, 
            string targetUserId, 
            string supervisorId)
        {
            var visited = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(targetUserId);

            while (queue.Count > 0)
            {
                var currentUserId = queue.Dequeue();

                if (visited.Contains(currentUserId))
                {
                    continue;
                }
                visited.Add(currentUserId);

                var supervisorRelation = await context.UserRelations
                    .FirstOrDefaultAsync(ur => ur.UserId == currentUserId);

                if (supervisorRelation != null)
                {
                    if (supervisorRelation.SupervisorId == supervisorId)
                    {
                        return true;
                    }

                    queue.Enqueue(supervisorRelation.SupervisorId);
                }
            }

            return false;
        }

        /// <summary>
        /// Get the full supervisor chain for a user (from bottom to top)
        /// </summary>
        public static async Task<List<string>> GetSupervisorChain(
            AppDbContext context, 
            string userId)
        {
            var chain = new List<string>();
            var visited = new HashSet<string>();
            var currentUserId = userId;

            while (currentUserId != null)
            {
                if (visited.Contains(currentUserId))
                {
                    break;
                }
                visited.Add(currentUserId);

                var relation = await context.UserRelations
                    .FirstOrDefaultAsync(ur => ur.UserId == currentUserId);

                if (relation != null)
                {
                    chain.Add(relation.SupervisorId);
                    currentUserId = relation.SupervisorId;
                }
                else
                {
                    break;
                }
            }

            return chain;
        }

        /// <summary>
        /// Get all subordinates (directly and indirectly) for a supervisor
        /// </summary>
        public static async Task<List<string>> GetAllSubordinateIds(
            AppDbContext context, 
            string supervisorId)
        {
            var allSubordinates = new HashSet<string>();
            var queue = new Queue<string>();
            queue.Enqueue(supervisorId);
            var visited = new HashSet<string>();

            while (queue.Count > 0)
            {
                var currentSupervisorId = queue.Dequeue();

                if (visited.Contains(currentSupervisorId))
                {
                    continue;
                }
                visited.Add(currentSupervisorId);

                var directSubordinates = await context.UserRelations
                    .Where(ur => ur.SupervisorId == currentSupervisorId)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                foreach (var subordinateId in directSubordinates)
                {
                    allSubordinates.Add(subordinateId);
                    queue.Enqueue(subordinateId);
                }
            }

            return allSubordinates.ToList();
        }
    }
}