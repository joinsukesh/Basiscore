function GetRandomLoadingMessage() {
    var lines = new Array(
        "The magic elves, with an abacus are working frantically in here.",
        "Happy elf and sad elf are talking about your data...",
        "All the elves are on a break now. You need to wait...",
        "Elf down! We're cloning the elf that was supposed to get your data...",
        "The elves are still drafting a plan for this task...",
        "Server down! The elves are trying to power it with a lemon and two electrodes...",
        "Testing data on a lab elf... ... ... We're going to need another one...",
        "Senior elf is testing RAM, CPU, Primary Disk...and your patience...",
        "The elves are scanning your hard drive for adult content...",
        "The elves are measuring the cable length to fetch your data...",        
        "The elves are searching for an answer to life, the universe, and everything...",
        "BE QUIET. We're trying to think, on how to get your data...",
        "We need more helpers.....So, the elves are making babies now...",
        "The elves have detected water in your C:\ drive. Spin dry commencing...",
        "The elves are adjusting data for your IQ...",
        "The elves have decided to add random changes to your data...",
        "The elves are fixing typos and grammar errors in your data...",
        "Lost some data in the Atlantic while shipping. The marine elf team is working on it...",
        "The elves are deleting unnecessary data from your hard drive...",
        "The elves are trying to decipher your intentions...",

    );

    //return lines[Math.round(Math.random() * (lines.length - 1))] + " Please wait...";
    return lines[Math.round(Math.random() * (lines.length - 1))];
}


