﻿using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options) : base(options) 
        {

        }
        public DbSet<Todo> Todos { get; set; }
    }
}
