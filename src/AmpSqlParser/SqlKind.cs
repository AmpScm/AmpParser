﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Amp.SqlParser
{
    public enum SqlKind
    {
        None=0,

        IdentifierToken,
        QuotedIdentifierToken,
        StringToken,

        // SQL1999 keyword tokens
        AbsoluteToken,
        ActionToken,
        AddToken,
        AdminToken,
        AfterToken,
        AggregateToken,
        Aliastoken,
        AllToken,
        AllocateToken,
        AlterToken,
        AndToken,
        AnyToken,
        AreToken,
        ArrayToken,
        AsToken,
        AscToken,
        AssertionToken,
        AtToken,
        AuthorizationToken,
        BeforeToken,
        BeginToken,
        BinaryToken,
        BitToken,
        BlobToken,
        BooleanToken,
        BothToken,
        BreadthToken,
        ByToken,
        CallToken,
        CascadeToken,
        CascadedToken,
        CaseToken,
        CastToken,
        CatalogToken,
        CharToken,
        CharacterToken,
        CheckToken,
        ClassToken,
        ClobToken,
        CloseToken,
        Collatetoken,
        CollationToken,
        ColumnToken,
        CommitToken,
        CompletionToken,
        ConnectToken,
        ConnectionToken,
        ConstraintToken,
        ConstraintsToken,
        ConstructorToken,
        ContinueToken,
        CorrespondingToken,
        CreateToken,
        CrossToken,
        CubeToken,
        CurrentToken,
        CurrentDateToken,
        CurrentPathToken,
        CurrentRoleToken,
        CurrentTimeToken,
        CurrentTimestampToken,
        CurrentUserToken,
        CursorToken,
        CycleToken,
        DataToken,
        DateToken,
        DayToken,
        DeallocateToken,
        DecToken,
        DecimalToken,
        DeclareToken,
        DefaultToken,
        DeferrableToken,
        DeferredToken,
        DeleteToken,
        DepthToken,
        DerefToken,
        DescToken,
        DescribeToken,
        DescriptorToken,
        DestroyToken,
        DestructorToken,
        DeterministicToken,
        DictionaryToken,
        DiagnosticsToken,
        DisconnectToken,
        DistinctToken,
        DomainToken,
        DoubleToken,
        DropToken,
        DynamicToken,
        EachToken,
        ElseToken,
        EndToken,
        EqualsToken,
        EscapeToken,
        EveryToken,
        ExceptToken,
        ExceptionToken,
        ExecuteToken,
        ExternalToken,
        FalseToken,
        FetchToken,
        FirstToken,
        FloatToken,
        ForToken,
        ForeignToken,
        FoundToken,
        FromToken,
        FreeToken,
        FullToken,
        FunctionToken,
        GeneralToken,
        GetToken,
        GlobalToken,
        GoToken,
        GotoToken,
        GrantToken,
        GroupToken,
        GroupingToken,
        HavingToken,
        HostToken,
        HourToken,
        IdentityToken,
        IgnoreToken,
        ImmediateToken,
        InToken,
        IndicatorToken,
        InitializeToken,
        InitiallyToken,
        InnerToken,
        InOutToken,
        InputToken,
        InsertToken,
        IntToken,
        IntegerToken,
        IntersectToken,
        IntervalToken,
        IntoToken,
        IsToken,
        IsolationToken,
        IterateToken,
        JoinToken,
        KeyToken,
        LanguageToken,
        LargeToken,
        LastToken,
        LateralToken,
        LeadingToken,
        LeftToken,
        LessToken,
        LevelToken,
        LikeToken,
        LimitToken,
        LocalToken,
        LocalTimeToken,
        LocalTimestampToken,
        LocatorToken,
        MapToken,
        MatchToken,
        MinuteToken,
        ModifiesToken,
        ModifyToken,
        ModuleToken,
        Monthtoken,
        NamesToken,
        NationalToken,
        NaturalToken,
        NCharToken,
        NClobToken,
        NewToken,
        NextToken,
        NoToken,
        NoneToken,
        NotToken,
        NullToken,
        NumericToken,
        ObjectToken,
        OfToken,
        OffToken,
        OldToken,
        OnToken,
        OnlyToken,
        OpenToken,
        OperationToken,
        OptionToken,
        OrToken,
        OrderToken,
        OrdinalityToken,
        OutToken,
        OuterToken,
        OutputToken,
        PadToken,
        ParameterToken,
        ParametersToken,
        PartialToken,
        PathToken,
        PostfixToken,
        PrecisionToken,
        PrefixToken,
        PreOrderToken,
        PrepareToken,
        PreserveToken,
        PrimaryToken,
        PriorToken,
        PrivilegesToken,
        ProcedureToken,
        PublicToken,
        ReadToken,
        ReadsToken,
        RealToken,
        RecursiveToken,
        RefToken,
        ReferencesToken,
        ReferencingToken,
        RelativeToken,
        RestrictToken,
        ResultToken,
        ReturnToken,
        ReturnsToken,
        RevokeToken,
        RightToken,
        RoleToken,
        RollBackToken,
        RollUpToken,
        RoutineToken,
        RowToken,
        RowsToken,
        SavepointToken,
        SchemaToken,
        ScrollToken,
        ScopeToken,
        SearchToken,
        SecondToken,
        SectionToken,
        SelectToken,
        SequenceToken,
        SessionToken,
        SessionUserToken,
        SetToken,
        SetsToken,
        SizeToken,
        SmallIntToken,
        SomeToken,
        SpaceToken,
        SpecificToken,
        SpecificTypeToken,
        SqlToken,
        SqlExceptionToken,
        SqlStateToken,
        SqlWarningToken,
        StartToken,
        StateToken,
        StatementToken,
        StaticToken,
        StructureToken,
        SystemUserToken,
        TableToken,
        TemporaryToken,
        TerminateToken,
        ThanToken,
        ThenToken,
        TimeToken,
        TimestampToken,
        TimeZoneHourToken,
        TimeZoneMinuteToken,
        ToToken,
        TrailingToken,
        TransactionToken,
        TranslationToken,
        TreatToken,
        TriggerToken,
        TrueToken,
        UnderToken,
        UnionToken,
        UniqueToken,
        UnknownToken,
        UnNestToken,
        UpdateToken,
        UsageToken,
        UserToken,
        UsingToken,
        ValueToken,
        ValuesToken,
        VarCharToken,
        VariableToken,
        VaryingToken,
        ViewToken,
        WhenToken,
        WheneverToken,
        WhereToken,
        WithToken,
        WithoutToken,
        WorkToken,
        WriteToken,
        YearToken,
        ZoneToken,


        




        BinaryOperatorToken,
        SemiColonOperatorToken,
        ColonOperatorToken,
        CommaToken,
        OpenParenToken,
        EndOfStream,
        ConcatToken,
        UnknownCharToken,
        DescendingToken,
        CloseParenToken,
        EqualOperatorToken,
        AsteriksToken,
        PlusOperatorToken,
        MinusOperatorToken,
        DivToken,
        PercentOperatorToken,
        LessThanToken,
        GreaterThanToken,
        LessThanOrEqualToken,
        GreaterThanOrEqualToken,
        ShiftLeftToken,
        ShiftRightToken,
        NotEqualToken,
        BetweenToken,
        ExistsToken,
        MinusSetToken,
        ReplaceToken,
        TruncToken,
        AccessToken,
        ExclusiveToken,
        NoAuditToken,
        NoCompressToken,
        FileToken,
        ShareToken,
        NoWaitToken,
        ArrayLenToken,
        GrandToken,
        NumberToken,
        SqlBufToken,
        SuccessfulToken,
        AuditToken,
        OfflineToken,
        SynonymToken,
        IdentifiedToken,
        SysDateToken,
        OnlineToken,
        IncrementToken,
        ClusterToken,
        IndexToken,
        InitialToken,
        PctFreeToken,
        UidToken,
        CommentToken,
        CompressToken,
        RawToken,
        RenameToken,
        ValidateToken,
        ResourceToken,
        LockToken,
        VarChar2Token,
        LongToken,
        RowIdToken,
        MaxExtendsToken,
        RowLabelToken,
        RowNumToken,
        ModeToken,
        OpenBracket,
        CloseBracket,
        DotToken,
        AmpersandToken,
        IntValueToken,
        DoubleValueToken,
        RaiseErrorToken,
        FillFactorToken,
        ReadTextToken,
        ReconfigureToken,
        FreeTextToken,
        FreeTextTableToken,
        ReplicationToken,
        BackupToken,
        RestoreToken,
        BreakToken,
        RevertToken,
        BrowseToken,
        BulkToken,
        HoldLockToken,
        RowCountToken,
        RowGuidColToken,
        IdentityInsertToken,
        RuleToken,
        CheckpointToken,
        IdentityColToken,
        SaveToken,
        IfToken,
        ClusteredToken,
        SecurityAuditToken,
        CoalesceToken,
        SemanticKeyPhraseTableToken,
        SemanticSimilarityDetailsTableToken,
        SemanticSimilarityTableToken,
        ComputeToken,
        ContainsToken,
        SetUserToken,
        ContainsTableToken,
        ShutdownToken,
        KillToken,
        ConvertToken,
        StatisticsToken,
        LineNoToken,
        LoadToken,
        TableSampleToken,
        MergeToken,
        TextSizeToken,
        NoCheckToken,
        NonClusteredToken,
        TopToken,
        TranToken,
        DatabaseToken,
        DbccToken,
        NullIfToken,
        TruncateToken,
        TryConvertToken,
        OffsetsToken,
        TSequalToken,
        DenyToken,
        OpenDataSourceToken,
        UniPivotToken,
        DiskToken,
        OpenQueryToken,
        OpenRowSetToken,
        UpdateTextToken,
        DistributedToken,
        OpenXmlToken,
        UseToken,
        DumpToken,
        OverToken,
        WaitForToken,
        ErrorLvlToken,
        PercentToken,
        PivotToken,
        PlanToken,
        WhileToken,
        ExecToken,
        WithinToken,
        PrintToken,
        WriteTextToken,
        ExitToken,
        AbortToken,
        AnalyzeToken,
        AttachToken,
        AutoIncrementToken,
        ConflictToken,
        DetachToken,
        DoToken,
        ExcludeToken,
        ExplainToken,
        FailToken,
        FilterToken,
        FollowingToken,
        GlobToken,
        GroupsToken,
        IndexedToken,
        InsteadToken,
        IsNullToken,
        NothingToken,
        NotNullToken,
        NullsToken,
        OffsetToken,
        OthersToken,
        PartitionToken,
        PragmaToken,
        PrecedingToken,
        QueryToken,
        RaiseToken,
        RangeToken,
        RegExpToken,
        ReIndexToken,
        ReleaseToken,
        TempToken,
        TiesToken,
        UnboundedToken,
        VacuumToken,
        VirtualToken,
        WindowToken,
        IncompleteStringToken,
        IncompleteQuotedIdentifierToken,
        OuterJoinToken,
    }
}
