﻿using Microsoft.Extensions.DependencyInjection;
using Senti.News.Core.Articles;
using Senti.News.Core.Queries;
using Senti.News.Core.Schedulers;

namespace Senti.News.Core;
public static class NewsTypeRegister
{
    public static void Register(this IServiceCollection services)
    {
        services.AddTransient<ImportRss>();
        services.AddTransient<CreateNews>();
        services.AddTransient<TextSentimentFactory>();
        services.AddTransient<ArticleFactory>();
        services.AddTransient<GetArticles>();
    }
}
