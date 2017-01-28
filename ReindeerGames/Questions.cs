using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReindeerGames
{
    /// <summary>
    /// Single question
    /// </summary>
    public class Question
    {
        /// <summary>
        /// Question to ask user
        /// </summary>
        public string QuestionText { get; }

        /// <summary>
        /// Possible answers. Answer in position 0 is always correct.
        /// </summary>
        public string[] Answers { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="question">Question to ask user</param>
        /// <param name="answers">Possible answers. Answer in position 0 is always correct.</param>
        public Question(string question, params string[] answers)
        {
            QuestionText = question;
            Answers = answers;
        }
    }

    /// <summary>
    /// Hardcoded collection of all of the questions
    /// </summary>
    public static class Questions
    {
        /// <summary>
        /// Array of all of the possible questions
        /// </summary>
        public static Question[] QuestionList =
        {
            new Question("Reindeer have very thick coats, how many hairs per square inch do they have?", new[]
            {
                "13,000",
                "1,200",
                "5,000",
                "700",
                "1,000",
                "120,000"
            }),
            new Question("The 1964 classic Rudolph The Red Nosed Reindeer was filmed in:", new[]
            {
                "Japan",
                "United States",
                "Finland",
                "Germany"
            }),
            new Question("Santas reindeer are cared for by one of the Christmas elves, what is his name?", new[]
            {
                "Wunorse Openslae",
                "Alabaster Snowball",
                "Bushy Evergreen",
                "Pepper Minstix"
            }),
            new Question("If all of Santas reindeer had antlers while pulling his Christmas sleigh, they would all be:", new[]
            {
                "Girls",
                "Boys",
                "Girls and boys",
                "No way to tell"
            }),
            new Question("What do Reindeer eat?", new[]
            {
                "Lichen",
                "Grasses",
                "Leaves",
                "Berries"
            }),
            new Question("What of the following is not true?", new[]
            {
                "Caribou live on all continents",
                "Both reindeer and Caribou are the same species",
                "Caribou are bigger than reindeer",
                "Reindeer live in Scandinavia and Russia"
            }),
            new Question("In what year did Rudolph make his television debut?", new[]
            {
                "1964",
                "1979",
                "2000",
                "1956"
            }),
            new Question("Who was the voice of Rudolph in the 1964 classic?", new[]
            {
                "Billie Mae Richards",
                "Burl Ives",
                "Paul Soles",
                "Lady Gaga"
            }),
            new Question("In 1939 what retailer used the story of Rudolph the Red Nose Reindeer?", new[]
            {
                "Montgomery Ward",
                "Sears",
                "Macys",
                "Kmart"
            }),
            new Question("Santa's reindeer named Donner was originally named what?", new[]
            {
                "Dunder",
                "Donny",
                "Dweedle",
                "Dreamy"
            }),
            new Question("Who invented the story of Rudolph?", new[]
            {
                "Robert May",
                "Johnny Marks",
                "Santa",
                "J.K. Rowling"
            }),
            new Question("In what location will you not find reindeer?", new[]
            {
                "North Pole",
                "Lapland",
                "Korvatunturi mountain",
                "Finland"
            }),
            new Question("What Makes Santa's Reindeer Fly?", new[]
            {
                "Magical Reindeer Dust",
                "Fusion",
                "Amanita muscaria",
                "Elves"
            }),
            new Question("Including Rudolph, how many reindeer hooves are there?", new[]
            {
                "36",
                "24",
                "16",
                "8"
            }),
            new Question("Santa only has one female reindeer. Which one is it?", new[]
            {
                "Vixen",
                "Clarice",
                "Cupid",
                "Cupid"
            }),
            new Question("In the 1964 classic Rudolph The Red Nosed Reindeer, what was the snowman narrators name?", new[]
            {
                "Sam",
                "Frosty",
                "Burl",
                "Snowy"
            }),
            new Question("What was Rudolph's father's name?", new[]
            {
                "Donner",
                "Dasher",
                "Blixen",
                "Comet"
            }),
            new Question("In the 1964 movie, What was the name of the coach of the Reindeer Games?", new[]
            {
                "Comet",
                "Blixen",
                "Donner",
                "Dasher"
            }),
            new Question(
                "In the 1964 movie, what is the name of the deer that Rudolph befriends at the reindeer games?", new[]
                {
                    "Fireball",
                    "Clarice",
                    "Jumper",
                    "Vixen"
                }),
            new Question("In the 1964 movie, How did Donner, Rudolph's father, try to hide Rudolph's nose?", new[]
            {
                "Black mud",
                "Bag",
                "Pillow case",
                "Sock"
            }),
            new Question("In the 1964 movie, what does the Misfit Elf want to be instead of a Santa Elf?", new[]
            {
                "Dentist",
                "Reindeer",
                "Toy maker",
                "Candlestick maker"
            }),
            new Question("In the 1964 movie,what was the Bumble's one weakness?", new[]
            {
                "Could not swim",
                "Always hungry",
                "Candy canes",
                "Cross eyed"
            }),
            new Question("In the 1964 movie, what is Yukon Cornelius really in search of?", new[]
            {
                "Peppermint",
                "Gold",
                "India",
                "Polar Bears"
            }),
            new Question("In the 1964 movie, why is the train on the Island of Misfit Toys?", new[]
            {
                "Square wheels",
                "No Engine",
                "Paint does not match",
                "It does not toot"
            }),
            new Question("In the 1964 movie, what is the name of the Jack in the Box?", new[]
            {
                "Charlie",
                "Sam",
                "Billy",
                "Jack"
            }),
            new Question("In the 1964 movie, why did Santa Claus almost cancel Christmas?", new[]
            {
                "Storm",
                "No snow",
                "No toys",
                "The Reindeer were sick"
            }),
            new Question("In the 1964 movie, what animal noise did the elf make to distract the Bumble?", new[]
            {
                "Oink",
                "Growl",
                "Bark",
                "Meow"
            }),
            new Question("In the 1964 movie, what is the name of the prospector?", new[]
            {
                "Yukon Cornelius",
                "Slider Sam",
                "Bumble",
                "Jack"
            }),
            new Question("How far do reindeer travel when they migrate?", new[]
            {
                "3000 miles",
                "700 miles",
                "500 miles",
                "0 miles"
            }),
            new Question("How fast can a reindeer run?", new[]
            {
                "48 miles per hour",
                "17 miles per hour",
                "19 miles per hour",
                "14 miles per hour"
            })
        };

    }
}
