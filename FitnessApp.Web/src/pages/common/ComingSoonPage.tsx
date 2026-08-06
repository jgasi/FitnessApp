interface ComingSoonPageProps {
  title: string;
  description: string;
}

export default function ComingSoonPage({ title, description }: ComingSoonPageProps) {
  return (
    <div className="coming-soon-page">
      <div className="page-card">
        <p className="eyebrow">Uskoro</p>
        <h1 className="title">{title}</h1>
        <p className="subtitle">{description}</p>
      </div>
    </div>
  );
}