using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ApartamentS.Models
{
    public static class LuceneSearch
    {
       
        private static string _luceneDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");

        private static FSDirectory _directoryTemp;

        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        private static void _addToLuceneIndex(Ad ad, IndexWriter writer)
        {
            //remove older index
            var searchQuery = new TermQuery(new Term("Id", ad.Id.ToString()));
            writer.DeleteDocuments(searchQuery);
            //add new index
            var doc = new Document();
            //add Lucene fields mapped to db fields
            doc.Add(new Field("Id", ad.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Author_Id", ad.Author_Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Author_Name", ad.Author_Name.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("DataCreat", ad.DataCreat.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Title", ad.Title.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description", ad.Description.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Address", ad.Address.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Phone", ad.Phone.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Price", ad.Price.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            //add entry to index
            writer.AddDocument(doc);
        }

        public static void AddUpdateLuceneIndex(IEnumerable<Ad> ads)
        {
            //init lucene
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var ad in ads)
                {
                    _addToLuceneIndex(ad, writer);
                }

                analyzer.Close();
                writer.Dispose();
            }
        }

        public static void AddUpdateLuceneIndex(Ad ad)
        {
            AddUpdateLuceneIndex(new List<Ad> {ad});
        }

        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }

        private static Ad _mapLuceneDocumentToData(Document doc)
        {
            return new Ad
            {
                Id = Convert.ToInt32(doc.Get("Id")),
                Author_Id = Convert.ToInt32(doc.Get("Author_Id")),
                Author_Name = doc.Get("Author_Name"),
                DataCreat = Convert.ToDateTime(doc.Get("DataCreat")),
                Title = doc.Get("Title"),
                Description = doc.Get("Description"),
                Address = doc.Get("Address"),
                Phone = doc.Get("Phone"),
                Price = Convert.ToInt32(doc.Get("Price"))
            };
        }

        private static IEnumerable<Ad> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }

        private static IEnumerable<Ad> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }


        private static IEnumerable<Ad> _search
    (string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<Ad>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Lucene.Net.Util.Version.LUCENE_30, new[] { "Title", "Description"}, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search
                    (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }

        public static IEnumerable<Ad> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<Ad>();
            var terms = input.Trim().Replace("-", " ").Split(' ').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);
            return _search(input, fieldName);
        }
    }
}