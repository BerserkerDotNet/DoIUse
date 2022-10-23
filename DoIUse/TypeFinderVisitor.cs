using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;

public class TypeFinderVisitor : ContextTrackingVisitor<AstNode>
{
    private readonly IDataExporter _dataExporter;
    private readonly string _typeToFind;

    public TypeFinderVisitor(IDataExporter dataExporter, string typeToFind)
    {
        _dataExporter = dataExporter;
        _typeToFind = typeToFind;
    }

    public override AstNode VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression) => CheckTypeMatch(typeReferenceExpression.Type);

    public override AstNode VisitIdentifierExpression(IdentifierExpression identifierExpression) => CheckTypeMatch(identifierExpression);

    public override AstNode VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression) => CheckTypeMatch(unaryOperatorExpression);

    public override AstNode VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression) => CheckTypeMatch(memberReferenceExpression);

    public override AstNode VisitInvocationExpression(InvocationExpression invocationExpression) => CheckTypeMatch(invocationExpression);

    public override AstNode VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression) => CheckTypeMatch(arrayCreateExpression);

    public override AstNode VisitAsExpression(AsExpression asExpression) => CheckTypeMatch(asExpression);

    public override AstNode VisitCastExpression(CastExpression castExpression) => CheckTypeMatch(castExpression);

    public override AstNode VisitIsExpression(IsExpression isExpression) => CheckTypeMatch(isExpression);

    public override AstNode VisitOutVarDeclarationExpression(OutVarDeclarationExpression outVarDeclarationExpression) => CheckTypeMatch(outVarDeclarationExpression);

    public override AstNode VisitCatchClause(CatchClause catchClause) => CheckTypeMatch(catchClause);

    public override AstNode VisitAttribute(ICSharpCode.Decompiler.CSharp.Syntax.Attribute attribute) => CheckTypeMatch(attribute);

    public override AstNode VisitConstraint(Constraint constraint)
    {
        foreach (var type in constraint.BaseTypes)
        {
            CheckTypeMatch(type);
        }

        return constraint;
    }

    public override AstNode VisitThrowExpression(ThrowExpression throwExpression) => CheckTypeMatch(throwExpression);

    public override AstNode VisitDeclarationExpression(DeclarationExpression declarationExpression) => CheckTypeMatch(declarationExpression.Type);

    public override AstNode VisitTypeOfExpression(TypeOfExpression typeOfExpression) => CheckTypeMatch(typeOfExpression);

    private AstNode CheckTypeMatch(AstNode node)
    {
        var resolvedType = node.GetResolveResult();
        if (resolvedType.Type.FullName == _typeToFind)
        {
            var path = currentMethod is not null ? currentMethod.FullName : currentTypeDefinition.FullName;
            _dataExporter.WriteMatch(new TypeReference(currentTypeDefinition.FullTypeName.Name, path));
        }

        return node;
    }
}