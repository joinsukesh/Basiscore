function GetRandomLoadingMessage() {
    var lines = new Array(
        "The minions, with an abacus are working frantically in here.",
        "Happy minion and sad minion are talking about your data...",
        "All the minions are on a break now. You need to wait...",
        "Minion down!! We're cloning the minion that was supposed to get your data...",
        "The minions are still drafting a plan for this task...",
        "Server down! The minions are trying to power it with a lemon and two electrodes...",
        "Testing data on a lab minion... ... ... We're going to need another one...",
        "Senior minion is testing RAM, CPU, Primary Disk...and your patience...",
        //"The minions are scanning your hard drive for adult content...",
        "The minions are measuring the cable length to fetch your data...",
        "The minions are searching for an answer to life, the universe, and everything...",
        "BE QUIET. We're trying to think, on how to get your data...",
        "We need more helpers.....So, the minions are making babies now...",
        "The minions have detected water in your C:\ drive. Spin dry commencing...",
        "The minions are adjusting data for your IQ...",
        "The minions have decided to add random changes to your data...",
        "The minions are fixing typos and grammar errors in your data...",
        "Lost some data in the Atlantic while shipping. The marine minion team is working on it...",
        "The minions are deleting unnecessary data from your hard drive...",
        "The minions are trying to decipher your intentions...",

    );

    return lines[GetRandomArrayValue(lines.length)];
}

function GetRandomMinionQuote() {
    var quotes = new Array( 
        "Never go to bed angry... Stay awake and plot revenge.",
        "If you hate yourself, remember that you are not alone. A lot of other people hate you too.",
        "If you keep following your dreams, they're going to file a restraining order.",
        "The best first thing to do in the morning is go right back to sleep.",
        "People who say they'll give their 110%, don't understand how percentages work.",
        "If your coffee order is more than four words, you are part of the problem",
        "The best things in life.... are actually really expensive.",
        "Every day is another oppurtunity to screw it up all again.",
        "Enjoy the good times, because something terrible is probably about to happen.",
        "If I have ever offended you, I'm not sorry and it's your fault.",
        "If life doesn't break you today, don't worry. It will try again tomorrow.",
        "Happiness is just sadness that hasn't happened yet.",
        "Every corpse on the Everest was once an extremely motivated person.",
        "Try hard and if you fail, don't worry because everyone expected that.",
        "Those who doubt your ability probably have a valid reason.",
        "The light at the end of the tunnel is a train",
        "Always believe that something wonderful... will probably never happen.",
        "Your family only loves you because they have to.",
        "If at first you don't succeed, it's probably never going to happen.",
        "Never put off until tomorrow when you can straight up cancel.",        
        "Some people remind me of old TV sets. You have to hit them a few times until they get the picture."

    );

    return quotes[GetRandomArrayValue(quotes.length)];
}

function GetRandomArrayValue(arrayLength) {
    return Math.round(Math.random() * (arrayLength - 1));
}