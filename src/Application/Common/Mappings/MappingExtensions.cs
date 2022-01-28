﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
        => PaginatedList<TDestination>.CreateAsync(queryable, pageNumber, pageSize);
}
