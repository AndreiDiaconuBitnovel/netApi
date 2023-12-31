﻿using Microsoft.EntityFrameworkCore;
using WebApplication2.DataDB;

namespace WebApplication2.Services.TranslateLanguageFolder
{
    public class TranslateLanguageService : ITranslateLanguageService
    {
        private readonly TeamDropDatabaseContext _testContext;

        public TranslateLanguageService(TeamDropDatabaseContext testContext)
        {
            _testContext = testContext;
        }
        public async Task<List<TranslateLanguage>> GetAllDataFromTranslateLanguageTable()
        {
            List<TranslateLanguage> res = await _testContext.TranslateLanguages.OrderBy(l => l.NameInternational).ToListAsync();
            return res;
        }
    }
}
