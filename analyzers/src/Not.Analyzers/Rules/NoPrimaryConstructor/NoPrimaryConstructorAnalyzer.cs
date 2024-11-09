using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Not.Analyzers.Base;

namespace Not.Analyzers.Rules.NoPrimaryConstructor;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NoPrimaryConstructorAnalyzer : AnalyzerBase
{
    public const string DIAGNOSTIC_ID = "NA0008";
    public NoPrimaryConstructorAnalyzer() : base(
        DIAGNOSTIC_ID,
        title: "Primary constructor usage detected",
        messageFormat: "Primary constructors are not allowed",
        description: "Use traditional constructors instead of primary constructors.")
    {
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ClassDeclaration, SyntaxKind.RecordDeclaration);
    }

    protected override void SafeAnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not TypeDeclarationSyntax typeDeclaration)
            return;

        if (typeDeclaration.ParameterList is not null)
        {
            var diagnostic = CreateDiagnostic(typeDeclaration.ParameterList.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
} 
