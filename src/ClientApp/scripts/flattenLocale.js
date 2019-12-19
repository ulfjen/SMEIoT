const fs = require('fs');

const DEFAULT_LANGUAGE = 'en';
const MESSAGES = './build/messages/en.json';
const WRITE_PATH = './src/locales';

const defaultPath = `${WRITE_PATH}/${DEFAULT_LANGUAGE}.json`;

const getFileContents = filename => {
  const file = fs.readFileSync(filename, 'utf8');
  const parsedFile = JSON.parse(file);

  return parsedFile;
}

try {
  if (fs.existsSync(defaultPath)) {
    fs.unlinkSync(defaultPath);
  }
} catch (error) {
  // eslint-disable-next-line no-console
  console.log(error);
}

const defaultMessages = getFileContents(MESSAGES)
  .reduce((collection, descriptor) => {
    collection[descriptor.id] = descriptor.defaultMessage;

    return collection;
  }, {});

fs.writeFileSync(
  defaultPath,
  JSON.stringify(defaultMessages, null, 2)
);
