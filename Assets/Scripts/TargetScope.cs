public enum TargetScope
{
    Caster,             // The monster using the skill
    RandomEnemy,       // Any single random living enemy
    AllEnemy,       // Every living enemy monster
    RandomFrontRowEnemy,     // A single random enemy in the front column
    RandomBackRowEnemy,      // A single random enemy in the back column
    AllFrontRowEnemy,        // All enemy monsters in the front column
    AllBackRowEnemy,          // All enemy monsters in the back column
    TopLaneEnemy,
    MidLaneEnemy,
    BotLaneEnemy,
    RandomAlly,
    AllAlly,
    RandomFrontRowAlly,
    RandomBackRowAlly,
    AllFrontRowAlly,        
    AllBackRowAlly,         
    TopLaneAlly,
    MidLaneAlly,
    BotLaneAlly,
    RandomFrontRowEnemyThenRandomBackRowEnemy,
    RandomBackRowEnemyThenRandomFrontRowEnemy,
    RandomFrontRowAllyThenRandomBackRowAlly,
    RandomBackRowAllyThenRandomFrontRowAlly

}