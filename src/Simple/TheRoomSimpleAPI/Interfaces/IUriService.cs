using System;
using TheRoomSimpleAPI.Model.Responses;

namespace TheRoomSimpleAPI.Interfaces
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}