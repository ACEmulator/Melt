using System;

namespace Melt
{
    /* 69 */
    enum HARBitmask
    {
        Undef_HARBitmask = 0x0,
        OpenHouse_HARBitmask = 0x1,
        AllegianceGuests_HARBitmask = 0x2,
        AllegianceStorage_HARBitmask = 0x4,
        Force32Bit_HARBitmask = 0x7FFFFFFF,
    };

    /* 70 */
    enum MovementTypes_Type
    {
        Invalid = 0x0,
        RawCommand = 0x1,
        InterpretedCommand = 0x2,
        StopRawCommand = 0x3,
        StopInterpretedCommand = 0x4,
        StopCompletely = 0x5,
        MoveToObject = 0x6,
        MoveToPosition = 0x7,
        TurnToObject = 0x8,
        TurnToHeading = 0x9,
        FORCE_Type_32_BIT = 0x7FFFFFFF,
    };

    /* 72 */
    enum StatType
    {
        Undef_StatType = 0x0,
        Int_StatType = 0x1,
        Float_StatType = 0x2,
        Position_StatType = 0x3,
        Skill_StatType = 0x4,
        String_StatType = 0x5,
        DataID_StatType = 0x6,
        InstanceID_StatType = 0x7,
        DID_StatType = 0x6,
        IID_StatType = 0x7,
        Attribute_StatType = 0x8,
        Attribute_2nd_StatType = 0x9,
        BodyDamageValue_StatType = 0xA,
        BodyDamageVariance_StatType = 0xB,
        BodyArmorValue_StatType = 0xC,
        Bool_StatType = 0xD,
        Int64_StatType = 0xE,
        Num_StatTypes = 0xF,
        FORCE_StatType_32_BIT = 0x7FFFFFFF,
    };

    /* 82 */
    enum AllegianceVersion
    {
        Undef_AllegianceVersion = 0x0,
        SpokespersonAdded_AllegianceVersion = 0x1,
        PoolsAdded_AllegianceVersion = 0x2,
        MotdAdded_AllegianceVersion = 0x3,
        ChatRoomIDAdded_AllegianceVersion = 0x4,
        BannedCharactersAdded_AllegianceVersion = 0x5,
        MultipleAllegianceOfficersAdded_AllegianceVersion = 0x6,
        Bindstones_AllegianceVersion = 0x7,
        AllegianceName_AllegianceVersion = 0x8,
        OfficersTitlesAdded_AllegianceVersion = 0x9,
        LockedState_AllegianceVersion = 0xA,
        ApprovedVassal_AllegianceVersion = 0xB,
        Newest_AllegianceVersion = 0xB,
        FORCE_AllegianceVersion_32_BIT = 0x7FFFFFFF,
    };

    /* 89 */
    enum UI_SELECTION_TYPE
    {
        SELECTION_TYPE_UNDEF = 0x0,
        SELECTION_TYPE_ITEM = 0x1,
        SELECTION_TYPE_COMPASS_ITEM = 0x2,
        SELECTION_TYPE_MONSTER = 0x3,
        SELECTION_TYPE_PLAYER = 0x4,
        SELECTION_TYPE_UNOPENED_CORPSE = 0x5,
        FORCE_UI_SELECTION_TYPE_32_BIT = 0x7FFFFFFF,
    };

    enum MoveType
    {
        MoveType_Invalid = 0x0,
        MoveType_Pass = 0x1,
        MoveType_Resign = 0x2,
        MoveType_Stalemate = 0x3,
        MoveType_Grid = 0x4,
        MoveType_FromTo = 0x5,
        MoveType_SelectedPiece = 0x6,
    };


    enum EnchantmentTypeEnum
    {
        Undef_EnchantmentType = 0x0,
        Attribute_EnchantmentType = 0x1,
        SecondAtt_EnchantmentType = 0x2,
        Int_EnchantmentType = 0x4,
        Float_EnchantmentType = 0x8,
        Skill_EnchantmentType = 0x10,
        BodyDamageValue_EnchantmentType = 0x20,
        BodyDamageVariance_EnchantmentType = 0x40,
        BodyArmorValue_EnchantmentType = 0x80,
        SingleStat_EnchantmentType = 0x1000,
        MultipleStat_EnchantmentType = 0x2000,
        Multiplicative_EnchantmentType = 0x4000,
        Additive_EnchantmentType = 0x8000,
        AttackSkills_EnchantmentType = 0x10000,
        DefenseSkills_EnchantmentType = 0x20000,
        Multiplicative_Degrade_EnchantmentType = 0x100000,
        Additive_Degrade_EnchantmentType = 0x200000,
        Vitae_EnchantmentType = 0x800000,
        Cooldown_EnchantmentType = 0x1000000,
        Beneficial_EnchantmentType = 0x2000000,
        StatTypes_EnchantmentType = 0xFF,
        FORCE_EnchantmentTypeEnum_32_BIT = 0x7FFFFFFF,
    };

 

    /* 111 */
    enum LandDefs_Direction
    {
        IN_VIEWER_BLOCK = 0x0,
        NORTH_OF_VIEWER = 0x1,
        SOUTH_OF_VIEWER = 0x2,
        EAST_OF_VIEWER = 0x3,
        WEST_OF_VIEWER = 0x4,
        NORTHWEST_OF_VIEWER = 0x5,
        SOUTHWEST_OF_VIEWER = 0x6,
        NORTHEAST_OF_VIEWER = 0x7,
        SOUTHEAST_OF_VIEWER = 0x8,
        UNKNOWN = 0x9,
        FORCE_Direction_32_BIT = 0x7FFFFFFF,
    };

    enum CharacterOption : uint
    {
        Undef_CharacterOption = 0x0,
        AutoRepeatAttack_CharacterOption = 0x2,
        IgnoreAllegianceRequests_CharacterOption = 0x4,
        IgnoreFellowshipRequests_CharacterOption = 0x8,
        AllowGive_CharacterOption = 0x40,
        ViewCombatTarget_CharacterOption = 0x80,
        ShowTooltips_CharacterOption = 0x100,
        UseDeception_CharacterOption = 0x200,
        ToggleRun_CharacterOption = 0x400,
        StayInChatMode_CharacterOption = 0x800,
        AdvancedCombatUI_CharacterOption = 0x1000,
        AutoTarget_CharacterOption = 0x2000,
        VividTargetingIndicator_CharacterOption = 0x8000,
        DisableMostWeatherEffects_CharacterOption = 0x10000,
        IgnoreTradeRequests_CharacterOption = 0x20000,
        FellowshipShareXP_CharacterOption = 0x40000,
        AcceptLootPermits_CharacterOption = 0x80000,
        FellowshipShareLoot_CharacterOption = 0x100000,
        SideBySideVitals_CharacterOption = 0x200000,
        CoordinatesOnRadar_CharacterOption = 0x400000,
        SpellDuration_CharacterOption = 0x800000,
        DisableHouseRestrictionEffects_CharacterOption = 0x2000000,
        DragItemOnPlayerOpensSecureTrade_CharacterOption = 0x4000000,
        DisplayAllegianceLogonNotifications_CharacterOption = 0x8000000,
        UseChargeAttack_CharacterOption = 0x10000000,
        AutoAcceptFellowRequest_CharacterOption = 0x20000000,
        HearAllegianceChat_CharacterOption = 0x40000000,
        UseCraftSuccessDialog_CharacterOption = 0x80000000,
        Default_CharacterOption = 0x50C4A54A,
        FORCE_CharacterOption_32_BIT = 0x7FFFFFFF,
    };

    enum CharacterOptions2
    {
        Undef_CharacterOptions2 = 0x0,
        PersistentAtDay_CharacterOptions2 = 0x1,
        DisplayDateOfBirth_CharacterOptions2 = 0x2,
        DisplayChessRank_CharacterOptions2 = 0x4,
        DisplayFishingSkill_CharacterOptions2 = 0x8,
        DisplayNumberDeaths_CharacterOptions2 = 0x10,
        DisplayAge_CharacterOptions2 = 0x20,
        TimeStamp_CharacterOptions2 = 0x40,
        SalvageMultiple_CharacterOptions2 = 0x80,
        HearGeneralChat_CharacterOptions2 = 0x100,
        HearTradeChat_CharacterOptions2 = 0x200,
        HearLFGChat_CharacterOptions2 = 0x400,
        HearRoleplayChat_CharacterOptions2 = 0x800,
        AppearOffline_CharacterOptions2 = 0x1000,
        DisplayNumberCharacterTitles_CharacterOptions2 = 0x2000,
        MainPackPreferred_CharacterOptions2 = 0x4000,
        LeadMissileTargets_CharacterOptions2 = 0x8000,
        UseFastMissiles_CharacterOptions2 = 0x10000,
        FilterLanguage_CharacterOptions2 = 0x20000,
        ConfirmVolatileRareUse_CharacterOptions2 = 0x40000,
        HearSocietyChat_CharacterOptions2 = 0x80000,
        ShowHelm_CharacterOptions2 = 0x100000,
        DisableDistanceFog_CharacterOptions2 = 0x200000,
        UseMouseTurning_CharacterOptions2 = 0x400000,
        ShowCloak_CharacterOptions2 = 0x800000,
        LockUI_CharacterOptions2 = 0x1000000,
        Default_CharacterOptions2 = 0x948700,
        FORCE_CharacterOptions2_32_BIT = 0x7FFFFFFF,
    };

    /* 123 */
    enum TradeStatusEnum
    {
        Undef_TradeStatus = 0x0,
        Pending_TradeStatus = 0x1,
        Open_TradeStatus = 0x2,
        WaitingToClose_TradeStatus = 0x4,
        FORCE_TradeStatusEnum_32_BIT = 0x7FFFFFFF,
    };

    /* 124 */
    enum SearchCommandExecuteErrors
    {
        SCEE_PATHNOTFOUND = 0x1,
        SCEE_MAXFILESFOUND = 0x2,
        SCEE_INDEXSEARCH = 0x3,
        SCEE_CONSTRAINT = 0x4,
        SCEE_SCOPEMISMATCH = 0x5,
        SCEE_CASESENINDEX = 0x6,
        SCEE_INDEXNOTCOMPLETE = 0x7,
    };

    enum SpellbookFilter
    {
        Undef_SpellbookFilter = 0x0,
        Creature_SpellbookFilter = 0x1,
        Item_SpellbookFilter = 0x2,
        Life_SpellbookFilter = 0x4,
        War_SpellbookFilter = 0x8,
        Level_1_SpellbookFilter = 0x10,
        Level_2_SpellbookFilter = 0x20,
        Level_3_SpellbookFilter = 0x40,
        Level_4_SpellbookFilter = 0x80,
        Level_5_SpellbookFilter = 0x100,
        Level_6_SpellbookFilter = 0x200,
        Level_7_SpellbookFilter = 0x400,
        Level_8_SpellbookFilter = 0x800,
        Level_9_SpellbookFilter = 0x1000,
        Void_SpellbookFilter = 0x2000,
        Default_SpellbookFilter = 0x3FFF,
        FORCE_SpellbookFilter_32_BIT = 0x7FFFFFFF,
    };

    enum SpellComponentCategory
    {
        Scarab_SpellComponentCategory = 0x0,
        Herb_SpellComponentCategory = 0x1,
        PowderedGem_SpellComponentCategory = 0x2,
        AlchemicalSubstance_SpellComponentCategory = 0x3,
        Talisman_SpellComponentCategory = 0x4,
        Taper_SpellComponentCategory = 0x5,
        Pea_SpellComponentCategory = 0x6,
        Num_SpellComponentCategories = 0x7,
        Undef_SpellComponentCategory = 0x8,
        FORCE_SpellComponentCategory_32_BIT = 0x7FFFFFFF,
    };

    /* 135 */
    enum HouseOp
    {
        Undef_HouseOp = 0x0,
        Buy_House = 0x1,
        Rent_House = 0x2,
        Force32Bit_House = 0x7FFFFFFF,
    };

    enum Target_Mode
    {
        TARGET_MODE_NONE = 0x0,
        TARGET_MODE_USE = 0x1,
        TARGET_MODE_EXAMINE = 0x2,
        TARGET_MODE_USE_TARGET = 0x3,
        FORCE_Target_Mode_32_BIT = 0x7FFFFFFF,
    };

    /* 149 */
    enum BODY_HEIGHT
    {
        UNDEF_BODY_HEIGHT = 0x0,
        HIGH_BODY_HEIGHT = 0x1,
        MEDIUM_BODY_HEIGHT = 0x2,
        LOW_BODY_HEIGHT = 0x3,
        NUM_BODY_HEIGHTS = 0x4,
        FORCE_BODY_HEIGHT32_BIT = 0x7FFFFFFF,
    };

    /* 150 */


    /* 151 */

    /* 154 */


    /* 158 */
    enum eAllegianceOfficerLevel
    {
        Undef_AllegianceOfficerLevel = 0x0,
        Speaker_AllegianceOfficerLevel = 0x1,
        Seneschal_AllegianceOfficerLevel = 0x2,
        Castellan_AllegianceOfficerLevel = 0x3,
        NumberOfOfficerTitles_AllegianceOfficerLevel = 0x3,
        FORCE_AllegianceOfficerLevel_32_BIT = 0x7FFFFFFF,
    };

    /* 159 */
    enum eAllegianceLockAction
    {
        Undef_AllegianceLockAction = 0x0,
        OffLock_AllegianceLockAction = 0x1,
        OnLock_AllegianceLockAction = 0x2,
        ToggleLock_AllegianceLockAction = 0x3,
        CheckLock_AllegianceLockAction = 0x4,
        CheckApproved_AllegianceLockAction = 0x5,
        ClearApproved_AllegianceLockAction = 0x6,
        NumberOfActions_AllegianceLockAction = 0x6,
        FORCE_AllegianceLockAction_32_BIT = 0x7FFFFFFF,
    };


    /* 181 */
    enum PlayerOption : uint
    {
        Invalid_PlayerOption = 0xFFFFFFFF,
        AutoRepeatAttack_PlayerOption = 0x0,
        IgnoreAllegianceRequests_PlayerOption = 0x1,
        IgnoreFellowshipRequests_PlayerOption = 0x2,
        IgnoreTradeRequests_PlayerOption = 0x3,
        DisableMostWeatherEffects_PlayerOption = 0x4,
        PersistentAtDay_PlayerOption = 0x5,
        AllowGive_PlayerOption = 0x6,
        ViewCombatTarget_PlayerOption = 0x7,
        ShowTooltips_PlayerOption = 0x8,
        UseDeception_PlayerOption = 0x9,
        ToggleRun_PlayerOption = 0xA,
        StayInChatMode_PlayerOption = 0xB,
        AdvancedCombatUI_PlayerOption = 0xC,
        AutoTarget_PlayerOption = 0xD,
        VividTargetingIndicator_PlayerOption = 0xE,
        FellowshipShareXP_PlayerOption = 0xF,
        AcceptLootPermits_PlayerOption = 0x10,
        FellowshipShareLoot_PlayerOption = 0x11,
        FellowshipAutoAcceptRequests_PlayerOption = 0x12,
        SideBySideVitals_PlayerOption = 0x13,
        CoordinatesOnRadar_PlayerOption = 0x14,
        SpellDuration_PlayerOption = 0x15,
        DisableHouseRestrictionEffects_PlayerOption = 0x16,
        DragItemOnPlayerOpensSecureTrade_PlayerOption = 0x17,
        DisplayAllegianceLogonNotifications_PlayerOption = 0x18,
        UseChargeAttack_PlayerOption = 0x19,
        UseCraftSuccessDialog_PlayerOption = 0x1A,
        HearAllegianceChat_PlayerOption = 0x1B,
        DisplayDateOfBirth_PlayerOption = 0x1C,
        DisplayAge_PlayerOption = 0x1D,
        DisplayChessRank_PlayerOption = 0x1E,
        DisplayFishingSkill_PlayerOption = 0x1F,
        DisplayNumberDeaths_PlayerOption = 0x20,
        DisplayTimeStamps_PlayerOption = 0x21,
        SalvageMultiple_PlayerOption = 0x22,
        HearGeneralChat_PlayerOption = 0x23,
        HearTradeChat_PlayerOption = 0x24,
        HearLFGChat_PlayerOption = 0x25,
        HearRoleplayChat_PlayerOption = 0x26,
        AppearOffline_PlayerOption = 0x27,
        DisplayNumberCharacterTitles_PlayerOption = 0x28,
        MainPackPreferred_PlayerOption = 0x29,
        LeadMissileTargets_PlayerOption = 0x2A,
        UseFastMissiles_PlayerOption = 0x2B,
        FilterLanguage_PlayerOption = 0x2C,
        ConfirmVolatileRareUse_PlayerOption = 0x2D,
        HearSocietyChat_PlayerOption = 0x2E,
        ShowHelm_PlayerOption = 0x2F,
        DisableDistanceFog_PlayerOption = 0x30,
        UseMouseTurning_PlayerOption = 0x31,
        ShowCloak_PlayerOption = 0x32,
        LockUI_PlayerOption = 0x33,
        TotalNumberOfPlayerOptions_PlayerOption = 0x34,
    };

    /* 182 */


    /* 183 */
    enum AppraisalProfile_ArmorEnchantment_BFIndex
    {
        BF_ARMOR_LEVEL = 0x1,
        BF_ARMOR_MOD_VS_SLASH = 0x2,
        BF_ARMOR_MOD_VS_PIERCE = 0x4,
        BF_ARMOR_MOD_VS_BLUDGEON = 0x8,
        BF_ARMOR_MOD_VS_COLD = 0x10,
        BF_ARMOR_MOD_VS_FIRE = 0x20,
        BF_ARMOR_MOD_VS_ACID = 0x40,
        BF_ARMOR_MOD_VS_ELECTRIC = 0x80,
        BF_ARMOR_MOD_VS_NETHER = 0x100,
        BF_ARMOR_LEVEL_HI = 0x10000,
        BF_ARMOR_MOD_VS_SLASH_HI = 0x20000,
        BF_ARMOR_MOD_VS_PIERCE_HI = 0x40000,
        BF_ARMOR_MOD_VS_BLUDGEON_HI = 0x80000,
        BF_ARMOR_MOD_VS_COLD_HI = 0x100000,
        BF_ARMOR_MOD_VS_FIRE_HI = 0x200000,
        BF_ARMOR_MOD_VS_ACID_HI = 0x400000,
        BF_ARMOR_MOD_VS_ELECTRIC_HI = 0x800000,
        BF_ARMOR_MOD_VS_NETHER_HI = 0x1000000,
        FORCE_ArmorEnchantment_BFIndex_32_BIT = 0x7FFFFFFF,
    };

    /* 184 */
    enum AppraisalProfile_WeaponEnchantment_BFIndex
    {
        BF_WEAPON_OFFENSE = 0x1,
        BF_WEAPON_DEFENSE = 0x2,
        BF_WEAPON_TIME = 0x4,
        BF_DAMAGE = 0x8,
        BF_DAMAGE_VARIANCE = 0x10,
        BF_DAMAGE_MOD = 0x20,
        BF_WEAPON_OFFENSE_HI = 0x10000,
        BF_WEAPON_DEFENSE_HI = 0x20000,
        BF_WEAPON_TIME_HI = 0x40000,
        BF_DAMAGE_HI = 0x80000,
        BF_DAMAGE_VARIANCE_HI = 0x100000,
        BF_DAMAGE_MOD_HI = 0x200000,
        FORCE_WeaponEnchantment_BFIndex_32_BIT = 0x7FFFFFFF,
    };

    /* 185 */
    enum AppraisalProfile_ResistanceEnchantment_BFIndex
    {
        BF_RESIST_SLASH = 0x1,
        BF_RESIST_PIERCE = 0x2,
        BF_RESIST_BLUDGEON = 0x4,
        BF_RESIST_FIRE = 0x8,
        BF_RESIST_COLD = 0x10,
        BF_RESIST_ACID = 0x20,
        BF_RESIST_ELECTRIC = 0x40,
        BF_RESIST_HEALTH_BOOST = 0x80,
        BF_RESIST_STAMINA_DRAIN = 0x100,
        BF_RESIST_STAMINA_BOOST = 0x200,
        BF_RESIST_MANA_DRAIN = 0x400,
        BF_RESIST_MANA_BOOST = 0x800,
        BF_MANA_CON_MOD = 0x1000,
        BF_ELE_DAMAGE_MOD = 0x2000,
        BF_RESIST_NETHER = 0x4000,
        BF_RESIST_SLASH_HI = 0x10000,
        BF_RESIST_PIERCE_HI = 0x20000,
        BF_RESIST_BLUDGEON_HI = 0x40000,
        BF_RESIST_FIRE_HI = 0x80000,
        BF_RESIST_COLD_HI = 0x100000,
        BF_RESIST_ACID_HI = 0x200000,
        BF_RESIST_ELECTRIC_HI = 0x400000,
        BF_RESIST_HEALTH_BOOST_HI = 0x800000,
        BF_RESIST_STAMINA_DRAIN_HI = 0x1000000,
        BF_RESIST_STAMINA_BOOST_HI = 0x2000000,
        BF_RESIST_MANA_DRAIN_HI = 0x4000000,
        BF_RESIST_MANA_BOOST_HI = 0x8000000,
        BF_MANA_CON_MOD_HI = 0x10000000,
        BF_ELE_DAMAGE_MOD_HI = 0x20000000,
        BF_RESIST_NETHER_HI = 0x40000000,
        FORCE_ResistanceEnchantment_BFIndex_32_BIT = 0x7FFFFFFF,
    };

    enum PowerBarMode
    {
        PBM_UNDEF = 0x0,
        PBM_COMBAT = 0x1,
        PBM_ADVANCED_COMBAT = 0x2,
        PBM_JUMP = 0x3,
        PBM_DDD = 0x4,
    };

    /* 193 */
    enum ShopMode
    {
        SHOP_MODE_UNDEF = 0x0,
        SHOP_MODE_NONE = 0x1,
        SHOP_MODE_BUY = 0x2,
        SHOP_MODE_SELL = 0x3,
        FORCE_ShopMode_32_BIT = 0x7FFFFFFF,
    };

    /* 200 */
    enum ChatTypeEnum
    {
        Undef_ChatTypeEnum = 0x0,
        Allegiance_ChatTypeEnum = 0x1,
        General_ChatTypeEnum = 0x2,
        Trade_ChatTypeEnum = 0x3,
        LFG_ChatTypeEnum = 0x4,
        Roleplay_ChatTypeEnum = 0x5,
        Society_ChatTypeEnum = 0x6,
        SocietyCelHan_ChatTypeEnum = 0x7,
        SocietyEldWeb_ChatTypeEnum = 0x8,
        SocietyRadBlo_ChatTypeEnum = 0x9,
        Olthoi_ChatTypeEnum = 0xA,
        FORCE_CHAT_TYPE_32_BIT = 0x7FFFFFFF,
    };



    /* 207 */
    enum SpellComponentType
    {
        Undef_SpellComponentType = 0x0,
        Power_SpellComponentType = 0x1,
        Action_SpellComponentType = 0x2,
        ConceptPrefix_SpellComponentType = 0x3,
        ConceptSuffix_SpellComponentType = 0x4,
        Target_SpellComponentType = 0x5,
        Accent_SpellComponentType = 0x6,
        Pea_SpellComponentType = 0x7,
        FORCE_SpellComponentType_32_BIT = 0x7FFFFFFF,
    };

    /* 228 */
    enum eChatTypes
    {
        eTextTypeDefault = 0x0,
        eTextTypeAllChannels = 0x1,
        eTextTypeSpeech = 0x2,
        eTextTypeSpeechDirect = 0x3,
        eTextTypeSpeechDirectSend = 0x4,
        eTextTypeSystemSvent = 0x5,
        eTextTypeCombat = 0x6,
        eTextTypeMagic = 0x7,
        eTextTypeChannel = 0x8,
        eTextTypeChannelCend = 0x9,
        eTextTypeSocialChannel = 0xA,
        eTextTypeSocialChannelSend = 0xB,
        eTextTypeEmote = 0xC,
        eTextTypeAdvancement = 0xD,
        eTextTypeAbuseChannel = 0xE,
        eTextTypeHelpChannel = 0xF,
        eTextTypeAppraisalChannel = 0x10,
        eTextTypeMagicCastingChannel = 0x11,
        eTextTypeAllegienceChannel = 0x12,
        eTextTypeFellowshipChannel = 0x13,
        eTextTypeWorld_broadcast = 0x14,
        eTextTypeCombatEnemy = 0x15,
        eTextTypeCombatSelf = 0x16,
        eTextTypeRecall = 0x17,
        eTextTypeCraft = 0x18,
        eTextTypeTotalNumChannels = 0x19,
    };

    /* 230 */
    enum eTradeListID
    {
        TradeListIDUndef = 0x0,
        TradeListIDSelf = 0x1,
        TradeListIDPartner = 0x2,
    };


    /* 332 */
    enum DetectionType
    {
        NoChangeDetection = 0x0,
        EnteredDetection = 0x1,
        LeftDetection = 0x2,
        FORCE_DetectionType_32_BIT = 0x7FFFFFFF,
    };

    /* 333 */
    enum PhysicsTimeStamp
    {
        POSITION_TS = 0x0,
        MOVEMENT_TS = 0x1,
        STATE_TS = 0x2,
        VECTOR_TS = 0x3,
        TELEPORT_TS = 0x4,
        SERVER_CONTROLLED_MOVE_TS = 0x5,
        FORCE_POSITION_TS = 0x6,
        OBJDESC_TS = 0x7,
        INSTANCE_TS = 0x8,
        NUM_PHYSICS_TS = 0x9,
        FORCE_PhysicsTimeStamp_32_BIT = 0x7FFFFFFF,
    };

    /* 342 */
    enum ObjectInfoEnum
    {
        DEFAULT_OI = 0x0,
        CONTACT_OI = 0x1,
        ON_WALKABLE_OI = 0x2,
        IS_VIEWER_OI = 0x4,
        PATH_CLIPPED_OI = 0x8,
        FREE_ROTATE_OI = 0x10,
        PERFECT_CLIP_OI = 0x40,
        IS_IMPENETRABLE = 0x80,
        IS_PLAYER = 0x100,
        EDGE_SLIDE = 0x200,
        IGNORE_CREATURES = 0x400,
        IS_PK = 0x800,
        IS_PKLITE = 0x1000,
        FORCE_ObjectInfoEnum_32_BIT = 0x7FFFFFFF,
    };

    /* 344 */
    enum PhysicsDesc_PhysicsDescInfo
    {
        CSetup = 0x1,
        MTABLE = 0x2,
        VELOCITY = 0x4,
        ACCELERATION = 0x8,
        OMEGA = 0x10,
        PARENT = 0x20,
        CHILDREN = 0x40,
        OBJSCALE = 0x80,
        FRICTION = 0x100,
        ELASTICITY = 0x200,
        TIMESTAMPS = 0x400,
        STABLE = 0x800,
        PETABLE = 0x1000,
        DEFAULT_SCRIPT = 0x2000,
        DEFAULT_SCRIPT_INTENSITY = 0x4000,
        POSITION = 0x8000,
        MOVEMENT = 0x10000,
        ANIMFRAME_ID = 0x20000,
        TRANSLUCENCY = 0x40000,
        FORCE_PhysicsDescInfo_32_BIT = 0x7FFFFFFF,
    };

    /* 374 */


    /* 375 */
    enum PublicWeenieDesc_BitfieldIndex
    {
        BF_OPENABLE = 0x1,
        BF_INSCRIBABLE = 0x2,
        BF_STUCK = 0x4,
        BF_PLAYER = 0x8,
        BF_ATTACKABLE = 0x10,
        BF_PLAYER_KILLER = 0x20,
        BF_HIDDEN_ADMIN = 0x40,
        BF_UI_HIDDEN = 0x80,
        BF_BOOK = 0x100,
        BF_VENDOR = 0x200,
        BF_PKSWITCH = 0x400,
        BF_NPKSWITCH = 0x800,
        BF_DOOR = 0x1000,
        BF_CORPSE = 0x2000,
        BF_LIFESTONE = 0x4000,
        BF_FOOD = 0x8000,
        BF_HEALER = 0x10000,
        BF_LOCKPICK = 0x20000,
        BF_PORTAL = 0x40000,
        BF_ADMIN = 0x100000,
        BF_FREE_PKSTATUS = 0x200000,
        BF_IMMUNE_CELL_RESTRICTIONS = 0x400000,
        BF_REQUIRES_PACKSLOT = 0x800000,
        BF_RETAINED = 0x1000000,
        BF_PKLITE_PKSTATUS = 0x2000000,
        BF_INCLUDES_SECOND_HEADER = 0x4000000,
        BF_BINDSTONE = 0x8000000,
        BF_VOLATILE_RARE = 0x10000000,
        BF_WIELD_ON_USE = 0x20000000,
        BF_WIELD_LEFT = 0x40000000,
        FORCE_BitfieldIndex_32_BIT = 0x7FFFFFFF,
    };

    /* 384 */


    /* 395 */
    enum ChessMoveResult : uint
    {
        NoMoveResult = 0x0,
        OKMoveToEmptySquare = 0x1,
        OKMoveToOccupiedSquare = 0x2,
        OKMoveEnPassant = 0x3,
        OKMoveMask = 0x3FF,
        OKMoveCHECK = 0x400,
        OKMoveCHECKMATE = 0x800,
        OKMovePromotion = 0x1000,
        OKMoveToEmptySquareCHECK = 0x401,
        OKMoveToOccupiedSquareCHECK = 0x402,
        OKMoveEnPassantCHECK = 0x403,
        OKMovePromotionCHECK = 0x1400,
        OKMoveToEmptySquareCHECKMATE = 0x801,
        OKMoveToOccupiedSquareCHECKMATE = 0x802,
        OKMoveEnPassantCHECKMATE = 0x803,
        OKMovePromotionCHECKMATE = 0x1800,
        BadMoveInvalidCommand = 0xFFFFFFFF,
        BadMoveNotPlaying = 0xFFFFFFFE,
        BadMoveNotYourTurn = 0xFFFFFFFD,
        BadMoveDirection = 0xFFFFFF9C,
        BadMoveDistance = 0xFFFFFF9B,
        BadMoveNoPiece = 0xFFFFFF9A,
        BadMoveNotYours = 0xFFFFFF99,
        BadMoveDestination = 0xFFFFFF98,
        BadMoveWouldClobber = 0xFFFFFF97,
        BadMoveSelfCheck = 0xFFFFFF96,
        BadMoveWouldCollide = 0xFFFFFF95,
        BadMoveCantCastleOutOfCheck = 0xFFFFFF94,
        BadMoveCantCastleThroughCheck = 0xFFFFFF93,
        BadMoveCantCastleAfterMoving = 0xFFFFFF92,
        BadMoveInvalidBoardState = 0xFFFFFF91,
        ForceChessMoveResult32Bit = 0x7FFFFFFF,
    };

    /* 396 */
    enum SKILL_CATEGORY
    {
        UNDEF_SKILL_CATEGORY = 0x0,
        WEAPON_SKILL_CATEGORY = 0x1,
        NONWEAPON_SKILL_CATEGORY = 0x2,
        MAGIC_SKILL_CATEGORY = 0x3,
        NUM_SKILL_CATEGORIES = 0x4,
        FORCE_SKILL_CATEGORY_32_BIT = 0x7FFFFFFF,
    };

    /* 399 */
    enum SECTION_3D
    {
        TOP_NW_SECTION = 0x0,
        MID_NW_SECTION = 0x1,
        BOT_NW_SECTION = 0x2,
        TOP_N_SECTION = 0x3,
        MID_N_SECTION = 0x4,
        BOT_N_SECTION = 0x5,
        TOP_NE_SECTION = 0x6,
        MID_NE_SECTION = 0x7,
        BOT_NE_SECTION = 0x8,
        W_SECTION = 0x9,
        CENTER_SECTION = 0xA,
        E_SECTION = 0xB,
        SW_SECTION = 0xC,
        LSW_SECTION = 0xD,
        RSW_SECTION = 0xE,
        TOP_S_SECTION = 0xF,
        MID_S_SECTION = 0x10,
        BOT_S_SECTION = 0x11,
        SE_SECTION = 0x12,
        LSE_SECTION = 0x13,
        RSE_SECTION = 0x14,
        NUM_3D_SECTIONS = 0x15,
        NOT_IN_3D_SECTION = 0x16,
    };

    /* 401 */
    enum DirectionNumber
    {
        Left = 0x0,
        ForwardAndLeft = 0x1,
        Forward = 0x2,
        ForwardAndRight = 0x3,
        Right = 0x4,
        RightAndBack = 0x5,
        Back = 0x6,
        LeftAndBack = 0x7,
        Knight1 = 0x8,
        Knight2 = 0x9,
        Knight3 = 0xA,
        Knight4 = 0xB,
        Knight5 = 0xC,
        Knight6 = 0xD,
        Knight7 = 0xE,
        Knight8 = 0xF,
        nDirections = 0x10,
        ForceDirectionNumber32Bit = 0x7FFFFFFF,
    };

    /* 402 */
    enum ChessPieceType
    {
        Empty = 0x0,
        Pawn = 0x1,
        Rook = 0x2,
        Castle = 0x2,
        Knight = 0x3,
        Bishop = 0x4,
        Queen = 0x5,
        King = 0x6,
        nPieceTypes = 0x7,
    };

    /* 403 */
    enum DropItemFlags
    {
        DROPITEM_FLAGS_NONE = 0x0,
        DROPITEM_IS_CONTAINER = 0x1,
        DROPITEM_IS_VENDOR = 0x2,
        DROPITEM_IS_SHORTCUT = 0x4,
        DROPITEM_IS_SALVAGE = 0x8,
        DROPITEM_IS_ALIAS = 0xE,
        FORCE_DROPITEM_FLAGS_32_BIT = 0x7FFFFFFF,
    };

    /* 408 */
    enum InventoryRequest
    {
        IR_NONE = 0x0,
        IR_MERGE = 0x1,
        IR_SPLIT = 0x2,
        IR_MOVE = 0x3,
        IR_PICK_UP = 0x4,
        IR_PUT_IN_CONTAINER = 0x5,
        IR_DROP = 0x6,
        IR_WIELD = 0x7,
        IR_VIEW_AS_GROUND_CONTAINER = 0x8,
        IR_GIVE = 0x9,
        IR_SHOP_EVENT = 0xA,
        FORCE_InventoryRequest_32_BIT = 0x7FFFFFFF,
    };

    /* 409 */
    enum NameType
    {
        NAME_SINGULAR = 0x0,
        NAME_PLURAL = 0x1,
        NAME_APPROPRIATE = 0x2,
        FORCE_NameType_32_BIT = 0x7FFFFFFF,
    };

    /* 415 */
    enum ShopEvent
    {
        SE_BUY = 0x0,
        SE_SELL = 0x1,
    };

    /* 416 */
    enum HousePanelTextColor
    {
        Normal_HousePanelTextColor = 0x0,
        RentPaid_HousePanelTextColor = 0x1,
        RentNotPaid_HousePanelTextColor = 0x2,
    };

    /* 417 */
    enum PlayerOptionPage_OptionListType
    {
        Header_OptionListType = 0x0,
        Seperator_OptionListType = 0x1,
        Boolean_OptionListType = 0x2,
        Float_OptionListType = 0x3,
        Menu_OptionListType = 0x4,
        BoolAndFloat_OptionListType = 0x5,
        FloatWithLabels_OptionListType = 0x6,
        BoolAndFloatWithLabels_OptionListType = 0x7,
        Bitfield64_OptionListType = 0x8,
    };

    /* 418 */
    enum ContractSortCriteria
    {
        eName = 0x0,
        eStatus = 0x1,
    };

    /* 419 */
    enum JournalSortCriteria
    {
        ePageNumber = 0x0,
        eTitle = 0x1,
        eLabel = 0x2,
        eTimer = 0x3,
    };

    /* 460 */

    /* 463 */
    enum ExperienceHandlingType
    {
        Undef_ExperienceHandlingType = 0x0,
        ApplyLevelMod_ExperienceHandlingType = 0x1,
        ShareWithFellows_ExperienceHandlingType = 0x2,
        AddFellowshipBonus_ExperienceHandlingType = 0x4,
        ShareWithAllegiance_ExperienceHandlingType = 0x8,
        ApplyToVitae_ExperienceHandlingType = 0x10,
        EarnsCP_ExperienceHandlingType = 0x20,
        ReducedByDistance_ExperienceHandlingType = 0x40,
        Monster_ExperienceHandlingType = 0x5F,
        NormalQuest_ExperienceHandlingType = 0x1A,
        NoShareQuest_ExperienceHandlingType = 0x10,
        PassupQuest_ExperienceHandlingType = 0x18,
        ReceivedFromFellowship_ExperienceHandlingType = 0x18,
        PPEarnedFromUse_ExperienceHandlingType = 0x7F,
        AdminRaiseXP_ExperienceHandlingType = 0x10,
        AdminRaiseSkillXP_ExperienceHandlingType = 0x10,
        ReceivedFromAllegiance_ExperienceHandlingType = 0x0,
        FORCE_ExperienceHandlingType_32_BIT = 0x7FFFFFFF,
    };

    /* 464 */
    enum CreatureAppraisalProfile_Enchantment_BFIndex
    {
        BF_STRENGTH = 0x1,
        BF_ENDURANCE = 0x2,
        BF_QUICKNESS = 0x4,
        BF_COORDINATION = 0x8,
        BF_FOCUS = 0x10,
        BF_SELF = 0x20,
        BF_MAX_HEALTH = 0x40,
        BF_MAX_STAMINA = 0x80,
        BF_MAX_MANA = 0x100,
        BF_STRENGTH_HI = 0x10000,
        BF_ENDURANCE_HI = 0x20000,
        BF_QUICKNESS_HI = 0x40000,
        BF_COORDINATION_HI = 0x80000,
        BF_FOCUS_HI = 0x100000,
        BF_SELF_HI = 0x200000,
        BF_MAX_HEALTH_HI = 0x400000,
        BF_MAX_STAMINA_HI = 0x800000,
        BF_MAX_MANA_HI = 0x1000000,
        FORCE_Enchantment_BFIndex_32_BIT = 0x7FFFFFFF,
    };

    /* 465 */


    /* 469 */
    enum FriendsUpdateType
    {
        FRIENDS_UPDATE = 0x0,
        FRIENDS_UPDATE_ADD = 0x1,
        FRIENDS_UPDATE_REMOVE = 0x2,
        FRIENDS_UPDATE_REMOVE_SILENT = 0x3,
        FRIENDS_UPDATE_ONLINE_STATUS = 0x4,
        FORCE_FriendsUpdateType_32_BIT = 0x7FFFFFFF,
    };

    /* 471 */
    enum EmitterType
    {
        Unknown_ET = 0x0,
        BirthratePerSec_ET = 0x1,
        BirthratePerMeter_ET = 0x2,
        FORCE_EmitterType_32_BIT = 0x7FFFFFFF,
    };

    /* 472 */
    enum ParticleType
    {
        Unknown_PT = 0x0,
        Still_PT = 0x1,
        LocalVelocity_PT = 0x2,
        ParabolicLVGA_PT = 0x3,
        ParabolicLVGAGR_PT = 0x4,
        Swarm_PT = 0x5,
        Explode_PT = 0x6,
        Implode_PT = 0x7,
        ParabolicLVLA_PT = 0x8,
        ParabolicLVLALR_PT = 0x9,
        ParabolicGVGA_PT = 0xA,
        ParabolicGVGAGR_PT = 0xB,
        GlobalVelocity_PT = 0xC,
        NumParticleType = 0xD,
        FORCE_ParticleType_32_BIT = 0x7FFFFFFF,
    };

    enum TargetStatus
    {
        Undef_TargetStatus = 0x0,
        Ok_TargetStatus = 0x1,
        ExitWorld_TargetStatus = 0x2,
        Teleported_TargetStatus = 0x3,
        Contained_TargetStatus = 0x4,
        Parented_TargetStatus = 0x5,
        TimedOut_TargetStatus = 0x6,
        FORCE_TargetStatus_32_BIT = 0x7FFFFFFF,
    };

    /* 479 */
    enum ObjCollisionProfile_Bitfield
    {
        Undef_OCPB = 0x0,
        Creature_OCPB = 0x1,
        Player_OCPB = 0x2,
        Attackable_OCPB = 0x4,
        Missile_OCPB = 0x8,
        Contact_OCPB = 0x10,
        MyContact_OCPB = 0x20,
        Door_OCPB = 0x40,
        Cloaked_OCPB = 0x80,
        FORCE_ObjCollisionProfile_Bitfield_32_BIT = 0x7FFFFFFF,
    };

    /* 504 */
    enum PortalLinkType
    {
        Undef_PortalLinkType = 0x0,
        LinkedLifestone_PortalLinkType = 0x1,
        LinkedPortalOne_PortalLinkType = 0x2,
        LinkedPortalTwo_PortalLinkType = 0x3,
        FORCE_PortalLinkType_32_BIT = 0x7FFFFFFF,
    };

    /* 505 */
    enum PortalRecallType
    {
        Undef_PortalRecallType = 0x0,
        LastLifestone_PortalRecallType = 0x1,
        LinkedLifestone_PortalRecallType = 0x2,
        LastPortal_PortalRecallType = 0x3,
        LinkedPortalOne_PortalRecallType = 0x4,
        LinkedPortalTwo_PortalRecallType = 0x5,
        FORCE_PortalRecallType_32_BIT = 0x7FFFFFFF,
    };

    /* 506 */
    enum PortalSummonType
    {
        Undef_PortalSummonType = 0x0,
        LinkedPortalOne_PortalSummonType = 0x1,
        LinkedPortalTwo_PortalSummonType = 0x2,
        FORCE_PortalSummonType_32_BIT = 0x7FFFFFFF,
    };

    /* 513 */


    /* 515 */
    enum ExperienceType
    {
        Undef_ExperienceType = 0x0,
        Attribute_ExperienceType = 0x1,
        Attribute2nd_ExperienceType = 0x2,
        TrainedSkill_ExperienceType = 0x3,
        SpecializedSkill_ExperienceType = 0x4,
        Level_ExperienceType = 0x5,
        Credit_ExperienceType = 0x6,
        FORCE_ExperienceType_32_BIT = 0x7FFFFFFF,
    };

    /* 519 */
    enum PublicWeenieDescPackHeader : uint
    {
        PWD_Packed_None = 0x0,
        PWD_Packed_PluralName = 0x1,
        PWD_Packed_ItemsCapacity = 0x2,
        PWD_Packed_ContainersCapacity = 0x4,
        PWD_Packed_Value = 0x8,
        PWD_Packed_Useability = 0x10,
        PWD_Packed_UseRadius = 0x20,
        PWD_Packed_Monarch = 0x40,
        PWD_Packed_UIEffects = 0x80,
        PWD_Packed_AmmoType = 0x100,
        PWD_Packed_CombatUse = 0x200,
        PWD_Packed_Structure = 0x400,
        PWD_Packed_MaxStructure = 0x800,
        PWD_Packed_StackSize = 0x1000,
        PWD_Packed_MaxStackSize = 0x2000,
        PWD_Packed_ContainerID = 0x4000,
        PWD_Packed_WielderID = 0x8000,
        PWD_Packed_ValidLocations = 0x10000,
        PWD_Packed_Location = 0x20000,
        PWD_Packed_Priority = 0x40000,
        PWD_Packed_TargetType = 0x80000,
        PWD_Packed_BlipColor = 0x100000,
        PWD_Packed_Burden = 0x200000,
        PWD_Packed_SpellID = 0x400000,
        PWD_Packed_RadarEnum = 0x800000,
        PWD_Packed_Workmanship = 0x1000000,
        PWD_Packed_HouseOwner = 0x2000000,
        PWD_Packed_HouseRestrictions = 0x4000000,
        PWD_Packed_PScript = 0x8000000,
        PWD_Packed_HookType = 0x10000000,
        PWD_Packed_HookItemTypes = 0x20000000,
        PWD_Packed_IconOverlay = 0x40000000,
        PWD_Packed_MaterialType = 0x80000000,
        PWD_Packed_ForceDWord = 0xFFFFFFFF,
    };

    /* 520 */
    enum PublicWeenieDescPackHeader2 : uint
    {
        PWD2_Packed_None = 0x0,
        PWD2_Packed_IconUnderlay = 0x1,
        PWD2_Packed_CooldownID = 0x2,
        PWD2_Packed_CooldownDuration = 0x4,
        PWD2_Packed_PetOwner = 0x8,
        PWD2_Packed_ForceDWord = 0xFFFFFFFF,
    };

    /* 528 */
    enum AllegianceIndex : uint
    {
        Undef_AllegianceIndex = 0x0,
        LoggedIn_AllegianceIndex = 0x1,
        Update_AllegianceIndex = 0x2,
        HasAllegianceAge_AllegianceIndex = 0x4,
        HasPackedLevel_AllegianceIndex = 0x8,
        MayPassupExperience_AllegianceIndex = 0x10,
        ForceDWord_AllegianceIndex = 0xFFFFFFFF,
    };

    /* 530 */
    enum CharCase
    {
        CASE_UPPER = 0x0,
        CASE_LOWER = 0x1,
        CASE_EITHER = 0x2,
        FORCE_CharCase_32_BIT = 0x7FFFFFFF,
    };

    /* 531 */
    enum ATTRIBUTE_CACHE_MASK
    {
        UNDEF_MASK = 0x0,
        STRENGTH_MASK = 0x1,
        ENDURANCE_MASK = 0x2,
        QUICKNESS_MASK = 0x4,
        COORDINATION_MASK = 0x8,
        FOCUS_MASK = 0x10,
        SELF_MASK = 0x20,
        HEALTH_MASK = 0x40,
        STAMINA_MASK = 0x80,
        MANA_MASK = 0x100,
        FORCE_ATTRIBUTE_CACHE_MASK_32_BIT = 0x7FFFFFFF,
    };

    /* 532 */


    /* 533 */
    enum GeneratorDefinedTimes
    {
        Undef_GeneratorDefinedTimes = 0x0,
        Dusk_GeneratorDefinedTimes = 0x1,
        Dawn_GeneratorDefinedTimes = 0x2,
        FORCE_GeneratorDefinedTimes_32_BIT = 0x7FFFFFFF,
    };

    /* 534 */


    /* 535 */

    /* 537 */
    enum SecurityLevelEnum
    {
        Undef_SecurityLevel = 0x0,
        Player_SecurityLevel = 0x0,
        Advocate1_SecurityLevel = 0x1,
        Advocate2_SecurityLevel = 0x2,
        Advocate3_SecurityLevel = 0x3,
        Advocate4_SecurityLevel = 0x4,
        Advocate5_SecurityLevel = 0x5,
        MaxAdvocate_SecurityLevel = 0x5,
        Sentinel1_SecurityLevel = 0x6,
        Sentinel2_SecurityLevel = 0x7,
        Sentinel3_SecurityLevel = 0x8,
        MaxSentinel_SecurityLevel = 0x8,
        Turbine_SecurityLevel = 0x9,
        Arch_SecurityLevel = 0xA,
        Admin_SecurityLevel = 0xB,
        Max_SecurityLevel = 0xB,
        FORCE_SecurityLevelEnum_32_BIT = 0x7FFFFFFF,
    };


    /* 539 */
    enum PlayerModulePackHeader
    {
        PM_Packed_None = 0x0,
        PM_Packed_ShortCutManager = 0x1,
        PM_Packed_SquelchList = 0x2,
        PM_Packed_MultiSpellLists = 0x4,
        PM_Packed_DesiredComps = 0x8,
        PM_Packed_ExtendedMultiSpellLists = 0x10,
        PM_Packed_SpellbookFilters = 0x20,
        PM_Packed_2ndCharacterOptions = 0x40,
        PM_Packed_TimeStampFormat = 0x80,
        PM_Packed_GenericQualitiesData = 0x100,
        PM_Packed_GameplayOptions = 0x200,
        PM_Packed_8_SpellLists = 0x400,
        FORCE_PlayerModulePackHeader_32_BIT = 0x7FFFFFFF,
    };
}