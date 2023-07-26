using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class GameConstants : MonoBehaviour{
    private static GameConstants instance;
    public static GameConstants Instance{get{return instance;}}

    [field: SerializeField] public float ChatBubbleTime{ get; private set; }
    [field: SerializeField] public float TutorialMsgTime{ get; private set; }
    /*************Questions************/
    public enum QuestionType{
        ChemistrySymbols,
        RockTypes,
        EnglishAnimalGroups,
    }
    
    private Dictionary<QuestionType, Question[]> qTypeToQuestionMap = new Dictionary<QuestionType, Question[]>();
    public Question[] GetQuestions(QuestionType questionType){
        if (qTypeToQuestionMap.ContainsKey(questionType))
            return qTypeToQuestionMap[questionType];
        else
            return null;
    }
    private Dictionary<QuestionType, string[]> qTypeToAnswerMap = new Dictionary<QuestionType, string[]>();
    public string[] GetAnswers(QuestionType questionType){
        if (qTypeToAnswerMap.ContainsKey(questionType))
            return qTypeToAnswerMap[questionType];
        else
            return null;
    }


    //stores a question to answer mapping for a specific type of question
    public class Question{
        public string question{get;}
        public string answer{get;}
        public string trivia{get;}
        public string triviaQuestion{get;}
        public string triviaAnswer{get;}
        public QuestionType questionType{get;}
        public Question(string question, string answer, string trivia, string triviaQuestion, string triviaAnswer, QuestionType questionType){
            this.question = question;
            this.answer = answer;
            this.trivia = trivia;
            this.triviaQuestion = triviaQuestion;
            this.triviaAnswer = triviaAnswer;
            this.questionType = questionType;
        }
    }


    /*************Difficulty************/
    [field: SerializeField] public static int Difficulty{get; set;}
    public float CoLearnerChanceToBeCorrect{get; private set;} = 0f;
    public int NumLifeLines{get; private set;} = 0;
    public int Level1NumIntialPlatforms{get; private set;} = 0;
    public int Level1IntervalOfIncrease{get; private set;} = 0;
    public int Level1PlatformIncreaseBy{get; private set;} = 0;
    public int Level1MaxNumPlatforms{get; private set;} = 5;
    public int FinalQuizNumIntialPlatforms{get; private set;} = 3;
    public int FinalQuizIntervalOfIncrease{get; private set;} = 0;
    public int FinalQuizPlatformIncreaseBy{get; private set;} = 0;
    public int FinalQuizMaxNumPlatforms{get; private set;} = 2;
    
    
    private void initialiseDifficultyVariables(){
        if (Difficulty == 0){
            CoLearnerChanceToBeCorrect = 0.2f;
            Level1NumIntialPlatforms = 2;
            Level1IntervalOfIncrease = 4;
            Level1PlatformIncreaseBy = 1;
            NumLifeLines = 5;
        }
        if (Difficulty == 1){
            CoLearnerChanceToBeCorrect = 0.50f;
            Level1NumIntialPlatforms = 2;
            Level1IntervalOfIncrease = 7;
            Level1PlatformIncreaseBy = 1;
            NumLifeLines = 4;
        }
        if (Difficulty == 2){
            CoLearnerChanceToBeCorrect = 0.80f;
            Level1NumIntialPlatforms = 2;
            Level1IntervalOfIncrease = 10;
            Level1PlatformIncreaseBy = 1;
            NumLifeLines = 3;
        }
        FinalQuizNumIntialPlatforms = 2;
    }

    private void TrueOrFalse(){
        string[] trues = {"Vanadium is named after the Old Norse name for Freyja"};
        string[] falses = {"Vanadium is named after the Old Norse name for Freyja"};
    }
    private void initaliseChemistryQsandAs(){
        string[] elementsNames = {"Hydrogen", "Helium", "Lithium", "Carbon", "Nitrogen", "Oxygen", 
        "Fluorine", "Sodium", "Aluminium", "Silicon", "Phosphorus", "Sulfur", "Chlorine", 
        "Potassium", "Calcium", "Titanium", "Vanadium", "Chromium", "Iron", 
        "Cobalt", "Nickel", "Copper", "Gallium", "Arsenic", "Bromine", 
        "Krypton", "Silver", "Iodine", "Tungsten", 
        "Gold", "Mercury", "Uranium", "Plutonium"};
        string[] elementsSymbols = {"H", "He", "Li", "C", "N", "O", 
        "F", "Na", "Al", "Si", "P", "S", "Cl",
        "K", "Ca","Ti", "V", "Cr", "Fe",
        "Co", "Ni", "Cu", "Ga", "As", "Br", 
        "Kr", "Ag", "I", "W", 
        "Au", "Hg", "U", "Pu"};
        string[] trivia = {"Hydrogen is the most common atom in the universe", "Helium is named after the sun", "The inherent voltage of lithium is 2.7 volts", "Diamonds are made of Carbon", "78% of the air is made up of Nitrogen", "The Melting Point of Oxygen is -218C",
        "Fluoride is added to our tap water", "Too much sodium in the blood is known as Hypernatremia", "Aluminium is the world's most abundant metal", "The melting point of silicon is 1410C", "Match heads are made of a type of Phosphorus", "Brimstone is an archaic term for Sulfur", "Chlorine is named after the greek word for green",
        "Potassium has a purple flame when burned", "The main constituent of eggshells are Calcium", "Titanium has the highest strength to weight ratio", "Vanadium is named after an Old Norse goddess Vanadis", "Chromium is the hardest metal", "70% of the iron in your body is in your hemoglobin",
        "Cobalt can treat cancer, but also kill you", "One german meaning of \"Nickel\" is \"False Copper\"", "Copper is naturally antibacterial", "Gallium has a melting point of 29C", "Arsenic is naturally present at high levels in the groundwater of several countries", "Bromine can be used as an alternative to chlorine in swimming pools",
        "Krypton was discovered partially by accident, that is why it is named after a Greek word \"Krypto\", means \"hidden\"", "Silver was valued higher than gold at one point in ancient Egypt", "Radioactive iodine can treat cancer", "Tungsten has the highest melting point of all metals", 
        "More than 90 percent of all gold ever used has been mined since 1848", "Mercury is the only metal on earth that is liquid at room temperature", "Uranium has the highest atomic weight of all naturally occurring elements", "When plutonium comes into contact with oxygen, it burns"};
        
        string[] triviaQuestions = {"Hydrogen is the second common atom in the universe", "Helium is named after the greek sun god Helios", "The inherent voltage of lithium is 2.7 volts", "Diamonds are made of Cobalt", "70% of the air is made up of Nitrogen", "The Melting Point of Oxygen is -218C",
        "Fluoride is added to our tap water", "To much sodium in the blood is known as Hyponatremia", "Aluminium is the world's most abundant element", "The melting point of silicon is 1410C", "Match heads are made of a type of Phosphorus", "Brimstone isn't real", "Chlorine is named after the greek word for blue",
        "Potassium has a crimson flame when burned", "The main constituent of eggshells is Calcium Bicarbonate", "Titanium has the highest strength to weight ratio", "Vanadium is named after the Old Norse name for Freyja", "Chromium is the hardest metal", "70% of the iron in your body is in your hemoglobin", 
        "Cobalt can treat cancer", "One german meaning of \"Nickel\" is \"False Copper\"", "Copper is naturally antibacterial", "Gallium has a melting point of 290C", "Arsenic is not found in nature", "Bromine can be used as an alternative to water in swimming pools",
        "Krypton is named after Kryptonite, Superman's greatest weakness", "Silver was valued higher than gold at one point in ancient Egypt", "Liquid iodine can treat cancer", "Tungsten has the highest melting point of all metals",
        "More than 90 percent of all gold ever used has been mined since 1848", "Mercury and Gallium are the only two metals on earth that are liquid at room temperature", "Uranium has the highest atomic weight of all naturally occurring elements", "When plutonium comes into contact with oxygen, it implodes"};
        string[] triviaAnswers = {"False", "True", "True", "False", "False", "True",
        "True", "False", "False", "True", "True", "False", "False", 
        "False", "False", "True", "True", "True", "True",
        "True", "True", "True", "False", "False", "False", 
        "False", "True", "False", "True",   
        "True", "False", "True", "True"};

        if (elementsNames.Length != elementsSymbols.Length || elementsNames.Length != trivia.Length || elementsNames.Length != triviaQuestions.Length || elementsNames.Length != triviaAnswers.Length) throw new System.Exception("GameConstants elements inconsistent");
        
        Question[] questions = new Question[elementsNames.Length];
        for (int i = 0 ; i < elementsNames.Length;i++){
            questions[i] = new Question("What is the chemical symbol for " + elementsNames[i] + "?", elementsSymbols[i], trivia[i], triviaQuestions[i], triviaAnswers[i], QuestionType.ChemistrySymbols);
        }
        qTypeToQuestionMap.Add(QuestionType.ChemistrySymbols, questions);
        qTypeToAnswerMap.Add(QuestionType.ChemistrySymbols, elementsSymbols);
    }
    private void initaliseRockTypeQsandAs(){
        string[] rockNames = {"Basalt", "Pumice", "Obsidian", "Scoria", "Granite", "Gabbro", "Diorite", 
        "Chalk", "Coal", "Limestone", "Sandstone", "Shale", "Rock Salt", "Ironstone",
        "Marble", "Gneiss", "Quartzite", "Schist", "Slate"};
        string[] rockTypes = {"Igneous", "Igneous", "Igneous", "Igneous", "Igneous", "Igneous", "Igneous",
         "Sedimentary", "Sedimentary", "Sedimentary", "Sedimentary", "Sedimentary", "Sedimentary", "Sedimentary", 
        "Metamorphic", "Metamorphic", "Metamorphic", "Metamorphic", "Metamorphic"};
        string[] trivia = {"Basalt can be found in the Antrim Derry Plateau", "Pumice looks likes a sponge", "In the rarest occasions obsidian can be found in mahogany, rainbow, gold, and green", "Scoria is commonly used in gas barbecue grills", "Granite is the oldest igneous rock in the world", "Gabbro rocks are found on earth, moon, mars, and many large asteroids", "Diorite has an appearance similar to that of a combination of salt and pepper", 
        "England's famous white cliffs of Dover are made from Chalk", "It takes roughly 1 million years to form Coal", "Cork's famous Blarney Stone is made of Limestone", "Sandstone has been used to make housewares since prehistoric times", "Shale is the most abundant of the sedimentary rocks", "Another name for rock salt is 'halite' which is the Greek word for salt", "There is no iron in ironstone",
        "Marble is highly porous in nature", "The name \"Gneiss\" originates from Germany", "Quartzite is resistant to acids and will not etch from acids such as vinegar or lemon juice.", "Schist is easy to break and its name derives from the Greek word schíxein meaning \"to split\"", "Slate has been used for roofing for over one thousand years"};
        
        string[] triviaQuestions = {"Basalt can be found in The Burren", "Pumice looks likes a sponge", "Obsidian can be found in a multitude of different colours", "Scoria is commonly used in gas barbecue grills", "Granite is the youngest igneous rock in the world", "Gabbro rocks are found on earth, moon, mars, and many large asteroids", "Diorite has salt and pepper in it", 
        "England's famous white cliffs of Dover are made from Chalk" , "It takes roughly 1 billion years to form Coal", "Cork's famous Blarney Stone is made of Limestone", "Sandstone has been used to make housewares since prehistoric times", "Shale is the most abundant of the sedimentary rocks", "Another name for rock salt is 'halite' which is the Greek word for salt", "Ironstone is made of Iron",
        "Marble looks like a sponge in nature", "The name \"Gneiss\" originates from Germany", "Quartzite is resistant to bases and will not etch from bases like caustic soda", "Schist is easy to break and its name derives from the Greek word schíxein meaning \"to split\"", "Slate has only recently been used for roofing"};
        string[] triviaAnswers = {"False", "True", "True", "True", "False", "True", "False",
        "True", "False", "True", "True", "True", "True", "False", 
        "False", "True", "False", "True", "False"};
        if (rockNames.Length != rockTypes.Length || rockNames.Length != trivia.Length || rockNames.Length != triviaQuestions.Length || rockNames.Length != triviaAnswers.Length) throw new System.Exception("GameConstants rocks inconsistent");
        
        Question[] questions = new Question[rockNames.Length];
        for (int i = 0 ; i < rockNames.Length;i++){
            questions[i] = new Question(rockNames[i] + " is what type of rock?", rockTypes[i], trivia[i], triviaQuestions[i], triviaAnswers[i], QuestionType.RockTypes);
        }
        qTypeToQuestionMap.Add(QuestionType.RockTypes, questions);
        qTypeToAnswerMap.Add(QuestionType.RockTypes, rockTypes);
    }
    private void initaliseEnglishQsandAs(){
        string[] animalGroup = {"Army", "Cauldron", "Caravan", "Coalition", "Mob", "Cast", "Business", "Stand", 
        "Tower", "Tribe", "Troubling", "Cloud", "Husk"};
        string[] animals = {"Ants", "Bats", "Camels", "Cheetahs", "Emus", 
        "Falcons", "Ferrets", "Flamingos", 
        "Giraffes", "Goats", "Goldfish", "Grasshoppers", "Hares"};
        string[] trivia = {"Ants have two stomachs", "Bats are the only flying mammal", "Camels are born without humps", "Only male cheetahs are social", "Australia lost a war against Emus", 
        "Falcons mate for life", "A ferret's normal heart rate is 200 to 250 beats per minute", "Flamingos get their pink color from their food", 
        "Giraffes can run as fast as 35 miles an hour over short distances", "Goat meat is the most consumed meat per capita worldwide", "Goldfish don't have stomachs", "Large grasshoppers can jump between 10 and 20 times its body length without the help of its wings", "Hares do not use burrows"};
        
        string[] triviaQuestions = {"Ants have no stomach", "Bats are the only flying mammal", "Camels are born with one hump, and later grow the other", "Only male cheetahs are social", "Australia lost a war against Emus", 
        "Female falcons kill their mate", "A ferret's normal heart rate is 200 to 250 beats per minute", "Flamingos get their pink color from the sun",
        "Giraffes can run as fast as 35 miles an hour over short distances", "Goat meat is the least consumed red meat per capita worldwide", "Goldfish have two stomachs", "Large grasshoppers can jump between 10 and 20 times its body length without the help of its wings", "Hares do not use burrows"};
        string[] triviaAnswers = {"False", "True", "False", "True", "True",
        "False", "True", "False", 
        "True", "False", "False", "True", "True"};
        if (animalGroup.Length != animals.Length || animalGroup.Length != trivia.Length || animalGroup.Length != triviaQuestions.Length || animalGroup.Length != triviaAnswers.Length) throw new System.Exception("GameConstants animals inconsistent");
        
        Question[] questions = new Question[animalGroup.Length];
        for (int i = 0 ; i < animalGroup.Length;i++){
            var firstLetter = animalGroup[i].Substring(0, 1);
            if (firstLetter == "A" || firstLetter == "E" || firstLetter == "I" || firstLetter == "O" || firstLetter == "U")
                questions[i] = new Question("An " + animalGroup[i] + " is a group of what animal", animals[i], trivia[i], triviaQuestions[i], triviaAnswers[i], QuestionType.EnglishAnimalGroups);
            else
                questions[i] = new Question("A " + animalGroup[i] + " is a group of what animal", animals[i], trivia[i], triviaQuestions[i], triviaAnswers[i], QuestionType.EnglishAnimalGroups);           
        }
        qTypeToQuestionMap.Add(QuestionType.EnglishAnimalGroups, questions);
        qTypeToAnswerMap.Add(QuestionType.EnglishAnimalGroups, animals);
    }

    public List<string> CompanionWelcomeMessages{get;} = new List<string>();
    public List<string> CompanionFinalQuizMessages{get;} = new List<string>();
    private void initialiseCompanionMessages(){
        CompanionWelcomeMessages.Add("Hello there, it looks like we're stuck here together");
        CompanionWelcomeMessages.Add("It's all about survival in this place\nEveryone for themselves");
        CompanionWelcomeMessages.Add("Only you and I can see everyone else,\nso they're gonna drop like flies soon");
        CompanionWelcomeMessages.Add("That being said, I'll help you " + NumLifeLines + " times");
        CompanionWelcomeMessages.Add("Press F whenever you need my advice");

        CompanionFinalQuizMessages.Add("Looks like its just the two of us left");
        CompanionFinalQuizMessages.Add("A promise is a promise, I'll still help you");
        CompanionFinalQuizMessages.Add("I only wish at least one of us makes it out of here");
    }
    public string getCompanionLifelineMessage(string answer){
        return "I think the answer is " + answer + ".";
    }


    private void Awake(){
        instance = this;
        initialiseDifficultyVariables();
        initaliseChemistryQsandAs();
        initaliseRockTypeQsandAs();
        initaliseEnglishQsandAs();
        initialiseCompanionMessages();
    }
}