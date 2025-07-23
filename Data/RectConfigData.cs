﻿using Microsoft.EntityFrameworkCore;
using UmatoMusume.Models;
using UmatoMusume.Utils;

namespace UmatoMusume.Data
{
    public class RectConfigData
    {
        private readonly UmatoDBContext _dbContext;

        public RectConfigData(UmatoDBContext _context)
        {
            _dbContext = _context;
            _dbContext.Database.EnsureCreated();
        }

        public async Task<RectConfig?> Get(string _rectName)
        {
            var rectConfig = await _dbContext.RectConfigs
                .AsNoTracking()
                .FirstOrDefaultAsync(rc => rc.RectName == _rectName);

            return rectConfig;
        }

        public async Task Upsert(RectConfig _config)
        {
            var check = await _dbContext.RectConfigs
                   .FirstOrDefaultAsync(rc => rc.RectName == _config.RectName);

            if (check != null)
            {
                check.X = _config.X;
                check.Y = _config.Y;
                check.Width = _config.Width;
                check.Height = _config.Height;
            }
            else
            {
                _dbContext.RectConfigs.Add(_config);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
