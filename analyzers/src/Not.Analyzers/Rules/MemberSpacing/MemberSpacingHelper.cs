using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Not.Analyzers.Members;
using static Not.Analyzers.Members.MemberKind;

namespace Not.Analyzers.Rules.MemberSpacing;

public class MemberSpacingHelper
{
    private static readonly ReadOnlyCollection<MemberKind[]> MemberGroups;
    private static readonly ReadOnlyCollection<MemberKind> AlwaysSeparated;

    static MemberSpacingHelper()
    {
        var memberGroups = new List<MemberKind[]>
        {
            new[] { PrivateConst, PrivateStaticReadonly, PublicConst, PublicStaticReadonly },
            new[] { PrivateReadonly, PrivateField, PublicField },
            new[] { PrivateCtor, ProtectedCtor, PublicCtor },
            new[] { PrivateProperty },
            new[] { AbstractProperty, AbstractMethod },
            new[] { ProtectedProperty, InternalProperty, PublicIndexDeclarator, PublicProperty },
        };
        var alwaysSeparated = new List<MemberKind>
        {
            PublicStaticMethod,
            PublicImplicitOperator,
            PublicOperator,
            PublicMethod,
            ProtectedMethod,
            InternalMethod,
            PrivateMethod,
            ProtectedClass,
            InternalClass,
            PrivateClass,
        };

        MemberGroups = new ReadOnlyCollection<MemberKind[]>(memberGroups);
        AlwaysSeparated = new ReadOnlyCollection<MemberKind>(alwaysSeparated);
    }

    public static bool RequiresBlankLineBetween(MemberKind first, MemberKind second)
    {
        if (AlwaysSeparated.Contains(first) || AlwaysSeparated.Contains(second))
        {
            return true;
        }
        foreach (var group in MemberGroups)
        {
            if (group.Contains(first) && group.Contains(second))
            {
                return false;
            }
        }

        return true;
    }

    public static bool HasLeadingBlankLine(MemberDeclarationSyntax member)
    {
        var trivia = member.GetLeadingTrivia();
        int newlineCount = 0;
        foreach (var t in trivia)
        {
            if (t.IsKind(SyntaxKind.EndOfLineTrivia))
                newlineCount++;
            else if (!t.IsKind(SyntaxKind.WhitespaceTrivia))
                newlineCount = 0;
        }
        return newlineCount >= 1; // One blank line means two newlines
    }
}
