using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            var tags = new Dictionary<string, Tag>
            {
                {"c#", new Tag{Id  =1, Name = "c#"} },
                {"angularjs", new Tag{Id=2, Name="angularjs"} },
                {"javascript", new Tag{Id=3, Name="javascript"} },
                {"linq", new Tag{Id=4, Name="linq"} },
                {"oop", new Tag{Id=5, Name="oop"} },
                {"nodejs", new Tag{Id=5, Name="nodejs"} }
            };


            var authors = new List<Author>
            {
                new Author{Id = 1, Name ="Mosh Hamedani"},
                new Author{Id = 2, Name = "Anthony Alicea"},
                new Author{Id = 3, Name = "Eric Wise", Courses = new Collection<Course>()},
                new Author{Id = 4, Name = "Tom Owsiak", Courses = new Collection<Course>()}
            };

            var courses = new List<Course>
            {
                new Course
                {
                    Id = 1, Name = "C# Basics", Author = authors[0], Description = "Description for C# Basics",
                    FullPrice = 46, Level = 1, Tags = new Collection<Tag>{tags["c#"]}
                },
                new Course
                {
                    Id = 2, Name = "C# Intermediate", Author = authors[0], FullPrice = 49,
                    Description = "Description for C# Intermediate", Level = 2, 
                    Tags = new Collection<Tag>()
                    {
                        tags["c#"],
                        tags["oop"]
                    }
                },
                new Course
                {
                    Id = 3,
                    Name = "C# Advanced",
                    Author = authors[0],
                    FullPrice = 69,
                    Description = "Description for C# Advanced",
                    Level = 3,
                    Tags = new Collection<Tag>()
                    {
                        tags["c#"]
                    }
                },
                new Course
                {
                    Id = 4,
                    Name = "Javascript: Understanding the Weird Parts",
                    Author = authors[1],
                    FullPrice = 149,
                    Description = "Description for Javascript",
                    Level = 2,
                    Tags = new Collection<Tag>()
                    {
                        tags["javascript"]
                    }
                },
                new Course
                {
                    Id = 5,
                    Name = "Learn and Understand AngularJS",
                    Author = authors[1],
                    FullPrice = 99,
                    Description = "Description for AngularJS",
                    Level = 2,
                    Tags = new Collection<Tag>()
                    {
                        tags["angularjs"]
                    }
                },
                new Course
                {
                    Id = 6,
                    Name = "Learn and Understand NodeJS",
                    Author = authors[1],
                    FullPrice = 149,
                    Description = "Description for NodeJS",
                    Level = 2,
                    Tags = new Collection<Tag>()
                    {
                        tags["nodejs"]
                    }
                },
                new Course
                {
                    Id = 7,
                    Name = "Programming for Complete Beginners",
                    Author = authors[2],
                    FullPrice = 45,
                    Description = "Description for Programming for Beginners",
                    Level = 1,
                    Tags = new Collection<Tag>()
                    {
                        tags["c#"]
                    }
                },
                new Course
                {
                    Id = 8,
                    Name = "A 16 Hour C# Course with Visual Studio 2013",
                    Author = authors[3],
                    FullPrice = 150,
                    Description = "Description 16 Hour Course",
                    Level = 1,
                    Tags = new Collection<Tag>()
                    {
                        tags["c#"]
                    }
                },
                new Course
                {
                    Id = 9,
                    Name = "Learn JavaScript Through Visual Studio 2013",
                    Author = authors[3],
                    FullPrice = 20,
                    Description = "Description Learn Javascript",
                    Level = 1,
                    Tags = new Collection<Tag>()
                    {
                        tags["javascript"]
                    }
                }
            };

            var query = from c in courses
                        where c.Level == 2 && c.Author.Id == 2
                        orderby c.Level descending, c.Name 
                        select new { Name = c.Name, Author = c.Author};

            foreach (var item in query)
            {
                Console.WriteLine(item.Name, item.Author);
            }
            Console.WriteLine();
            Console.WriteLine();

            var query1 = courses.Where(c => c.Level == 2 && c.Author.Id == 2).OrderByDescending(c => c.Level).ThenBy(c => c.Name)
                                .Select(c => new { Name = c.Name, Author = c.Author });

            foreach (var item in query)
            {
                Console.WriteLine(item.Name, item.Author);
            }
            Console.WriteLine();
            Console.WriteLine();


            var groupQuery = from c in courses
                             group c by c.Level
                             into g
                             select new {Level = g.Key, Courses = from course in g select course};

            var groupQuery1 = courses.GroupBy(c => c.Level).Select(g => new { Level = g.Key, Courses = g.Select(c => c) });


            foreach (var group in groupQuery)
            {
                Console.WriteLine("Level of the course - {0}:", group.Level);
                foreach (var item in group.Courses)
                {
                    Console.WriteLine(item.Name);
                }
                Console.WriteLine();
            }

            var join = from c in courses
                        join a in authors on c.Author.Id equals a.Id
                        select new { CourseName = c.Name, Author = a.Name };

            var join1 = courses.Join(authors, c => c.Author.Id, a => a.Id, (course, author) => new { CourseName = course.Name, AuthorName = author.Name});


            var gjoin = from a in authors
                        join c in courses on a.Id equals c.Author.Id into g
                        select new { AuthorName = a.Name, CoursesCount = g.Count() };

            var gjoin1 = authors.GroupJoin(courses, a => a.Id, c => c.Author.Id, (a, c) => new { AuthorName = a.Name, CoursesCount = c.Count() });


            foreach (var item in gjoin)
            {
                Console.WriteLine("Author {0} has {1} course(-s)", item.AuthorName, item.CoursesCount);
            }
            Console.WriteLine();
            Console.WriteLine();

            var certainCourses = courses.Skip(3).Take(4);
            foreach (var course in certainCourses)
            {
                Console.WriteLine("Course {0}. Author {1}. Price {2}.", course.Name, course.Author, course.FullPrice);
            }

            var courseTags = courses.SelectMany(c => c.Tags);
            foreach (var tag in courseTags)
            {
                Console.WriteLine("{0}", tag.Name);
            }

            var v = courses.OrderBy(c => c.Level).FirstOrDefault(c => c.FullPrice > 100);
            var f = courses.SingleOrDefault(c => c.Id == 2);

            var all = courses.All(c => c.FullPrice > 10); 
            var any = courses.Any(c => c.Level == 1);
            var count = courses.Where(c => c.Level == 2).Count();
            var max = courses.Max(c => c.FullPrice);
            var cnt = courses.GroupBy(c => c.Author.Name).Select(g => new { AuthorName = g.Key, OverallCoursesCost = g.Sum(c => c.FullPrice) });
            var min = courses.Min(c => c.FullPrice);
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}", all, any, max, min, count);
            foreach (var item in cnt)
            {
                Console.WriteLine("{0} {1}", item.AuthorName, item.OverallCoursesCost);
            }

        }
    }
}
